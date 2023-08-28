using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TilesWaveCollapse.Generator;

public class WfcGenerator
{
    /// <summary>
    /// Output map of tiles
    /// </summary>
    public Tile?[,] Tilemap { get; private set; }
    /// <summary>
    /// Function that executes when some tile is placed
    /// </summary>
    public Action<int, int> TileSetted { get; set; } = (x, y) => { };
    /// <summary>
    /// All tiles including rotations
    /// </summary>
    public List<Tile> Tiles { get; private set; }

    private readonly HashSet<int>[,] wave;
    private readonly int w, h;
    private readonly Random rnd;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="_width">horizontal size of field</param>
    /// <param name="_height">vertical size of field</param>
    public WfcGenerator(int _width, int _height)
    {
        w = _width;
        h = _height;
        Tiles = new();
        rnd = new();
        Tilemap = new Tile[h, w];
        wave = new HashSet<int>[h, w];
    }
    /// <summary>
    /// Initialize fields with values
    /// </summary>
    public void InitField()
    {
        for (int i = 0; i < h; i++)
            for (int j = 0; j < w; j++)
            {
                Tilemap[i, j] = null;
                wave[i, j] = new();
                for (int k = 0; k < Tiles.Count; k++)
                    wave[i, j].Add(k);
            }
    }
    /// <summary>
    /// Experemental function
    /// </summary>
    public void SetBordersEmpty()
    {
        SetTile(0, 0, 5);
        SetTile(0, 1, 7);
        SetTile(h - 1, w - 1, 5);
        SetTile(h - 2, w - 1, 7);
        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
                if (Tilemap[j, i] is null && (i == 0 || j == 0 || i == w - 1 || j == h - 1))
                    SetTile(j, i, 0);
    }
    /// <summary>
    /// Adds tile to the list of tiles
    /// </summary>
    /// <param name="_tile">tile to add</param>
    /// <param name="_includeRotation">true if this tile should be rotated</param>
    public void AddTile(Tile _tile, bool _includeRotation = true)
    {
        if (_includeRotation)
            Tiles.AddRange(_tile.GetAllRotations());
        else
            Tiles.Add(_tile);
    }
    /// <summary>
    /// Sets initial tile to position
    /// </summary>
    /// <param name="x">position x</param>
    /// <param name="y">position y</param>
    /// <param name="tile">tile index in Tilelist to add</param>
    private void SetTile(int x, int y, int? tile = null)
    {
        int n = tile ?? wave[x, y].ElementAt(rnd.Next(wave[x, y].Count));
        Tilemap[x, y] = Tiles[n];
        wave[x, y].Clear();
        wave[x, y].Add(n);
        TileSetted(x, y);
        CountEntropy(x, y);
    }
    /// <summary>
    /// Generates tilemap
    /// </summary>
    public void Generate()
    {
        int x = rnd.Next(h),
            y = rnd.Next(w);
        x = y = 1;
        do
        {
            SetTile(x, y);
        } while (FindMinEntropy(out x, out y));
    }
    /// <summary>
    /// Finds position with monimal entropy
    /// </summary>
    /// <param name="x">x position</param>
    /// <param name="y">y position</param>
    /// <returns>true if min entropy exists</returns>
    bool FindMinEntropy(out int x, out int y)
    {
        int minEntropy = int.MaxValue;
        x = y = -1;
        List<(int, int)> p = new();
        for (int i = 0; i < h; i++)
            for (int j = 0; j < w; j++)
                if (Tilemap[i, j] is null && wave[i, j].Count < minEntropy)
                {
                    minEntropy = wave[i, j].Count;
                    x = i;
                    y = j;
                    p.Clear();
                    p.Add((x, y));
                }
                else if (Tilemap[i, j] is null && wave[i, j].Count == minEntropy)
                    p.Add((i, j));
        if (x != -1)
            (x, y) = p[rnd.Next(p.Count)];
        return x != -1;
    }
    /// <summary>
    /// Recursive function to count wave variable entropies (of adjacent cells)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="map"></param>
    void CountEntropy(int x, int y, Dictionary<int, HashSet<int>>? map = null)
    {
        map ??= new();
        if (map.ContainsKey(x))
        {
            if (map[x].Contains(y))
                return;
            map[x].Add(y);
        }
        else
            map[x] = new() { y };

        if (x > 0 && Tilemap[x - 1, y] is null &&
            wave[x - 1, y].RemoveWhere(t => wave[x, y].All(fr => !Tiles[t].CanConnectTo(Tiles[fr], Side.Bottom))) != 0)
            if (wave[x - 1, y].Count == 1)
                SetTile(x - 1, y);
            else
                CountEntropy(x - 1, y, map);
        if (y > 0 && Tilemap[x, y - 1] is null &&
            wave[x, y - 1].RemoveWhere(t => wave[x, y].All(fr => !Tiles[t].CanConnectTo(Tiles[fr], Side.Right))) != 0)
            if (wave[x, y - 1].Count == 1)
                SetTile(x, y - 1);
            else
                CountEntropy(x, y - 1, map);
        if (x < h - 1 && Tilemap[x + 1, y] is null &&
            wave[x + 1, y].RemoveWhere(t => wave[x, y].All(fr => !Tiles[t].CanConnectTo(Tiles[fr], Side.Top))) != 0)
            if (wave[x + 1, y].Count == 1)
                SetTile(x + 1, y);
            else
                CountEntropy(x + 1, y, map);
        if (y < w - 1 && Tilemap[x, y + 1] is null &&
            wave[x, y + 1].RemoveWhere(t => wave[x, y].All(fr => !Tiles[t].CanConnectTo(Tiles[fr], Side.Left))) != 0)
            if (wave[x, y + 1].Count == 1)
                SetTile(x, y + 1);
            else
                CountEntropy(x, y + 1, map);

        map[x].Remove(y);
    }
    public int[,] MAZE()
    {
        int yes = int.MaxValue, no = int.MinValue;
        //int yes = 1, no = 0;
        int[,] answ = new int[3*h+2, 3*w+2];
        for (int i = 0; i < 3 * w + 2; i++)
            for (int j = 0; j < 3 * h + 2; j++)
                answ[j, i] = no;

        for (int i = 0; i < h; i++)
            for (int j = 0; j < w; j++)
            {
                if (Tilemap[i, j].img == 0)
                    continue;
                answ[1 + i * 3 + 1, 1 + (j) * 3 + 1] = yes;
                if (Tilemap[i, j].SidesTypes[0, 0] == 1)
                    answ[1 + (i) * 3, 1 + (j) * 3 + 1] = yes;
                if (Tilemap[i, j].SidesTypes[1, 0] == 1)
                    answ[1 + (i) * 3 + 1, 1 + (j) * 3 + 2] = yes;
                if (Tilemap[i, j].SidesTypes[2, 0] == 1)
                    answ[1 + (i) * 3 + 2, 1 + (j) * 3 + 1] = yes;
                if (Tilemap[i, j].SidesTypes[3, 0] == 1)
                    answ[1 + (i) * 3 + 1, 1 + (j) * 3] = yes;
            }
        //FileStream f = new("C:\\Users\\volde\\Desktop\\a.txt", FileMode.OpenOrCreate);
        //for (int i = 0; i < 3 * h + 2; i++)
        //{
        //    for (int j = 0; j < 3 * w + 2; j++)
        //    {
        //        f.WriteByte((byte)(answ[i, j] == 1 ? '1' : '0'));
        //    }
        //    f.WriteByte((byte)('\n'));
        //}
        //f.Close();
        return answ;
    }
}
