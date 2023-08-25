using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TilesWaveCollapse.Generator;

public class WfcGenerator
{
    public Tile?[,] Tilemap { get; private set; }
    public Action<int, int> TileSetted { get; set; } = (x, y) => { };

    public List<Tile> Tiles { get; private set; }
    private readonly HashSet<int>[,] wave;
    private readonly int w, h;
    private readonly Random rnd;

    public WfcGenerator(int _width, int _height)
    {
        w = _width;
        h = _height;
        Tiles = new();
        rnd = new();
        Tilemap = new Tile[h, w];
        wave = new HashSet<int>[h, w];
    }

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

    public void AddTile(Tile _tile, bool _includeRotation = true)
    {
        if (_includeRotation)
            Tiles.AddRange(_tile.GetAllRotations());
        else
            Tiles.Add(_tile);
    }
    private void SetTile(int x, int y, int? tile = null)
    {
        Tilemap[x, y] = Tiles[tile ?? wave[x, y].First()];
        wave[x, y].Clear();
        TileSetted(x, y);
        CountEntropy(x, y);
    }
    public void Generate()
    {
        int x = rnd.Next(h),
            y = rnd.Next(w);
        do
        {
            SetTile(x, y, wave[x, y].ElementAt(rnd.Next(wave[x, y].Count)));
        } while (FindMinEntropy(out x, out y));
    }
    bool FindMinEntropy(out int x, out int y)
    {
        int minEntropy = int.MaxValue;
        x = y = -1;
        bool exist = false;
        for (int i = 0; i < h; i++)
            for (int j = 0; j < w; j++)
                if (Tilemap[i, j] is null && wave[i, j].Count < minEntropy)
                {
                    minEntropy = wave[i, j].Count;
                    exist = true;
                    x = i;
                    y = j;
                }
        return exist;
    }

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
}
