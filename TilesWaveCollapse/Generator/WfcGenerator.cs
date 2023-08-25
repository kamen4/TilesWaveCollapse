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
    public int TilesCount => tiles.Count;
    public Action<int, int> TileSetted = (x, y) => { };

    private readonly HashSet<int>[,] wave;
    private readonly List<Tile> tiles;
    private readonly int w, h;
    private readonly Random rnd;

    public WfcGenerator(int _width, int _height)
    {
        w = _width;
        h = _height;
        tiles = new();
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
                for (int k = 0; k < tiles.Count; k++)
                    wave[i, j].Add(k);
            }
    }

    public void AddTile(Tile _tile, bool _includeRotation = true)
    {
        if (_includeRotation)
            tiles.AddRange(_tile.GetAllRotations());
        else
            tiles.Add(_tile);
    }
    private void SetTile(int x, int y, int tile)
    {
        wave[x, y].Clear();
        Tilemap[x, y] = tiles[tile];
        CountEntropy(x, y, new());
        TileSetted(x, y);
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

    void CountEntropy(int x, int y, Dictionary<int, HashSet<int>> map)
    {
        if (Tilemap[x, y] is null)
            return;
        if (map.ContainsKey(x))
        {
            if (map[x].Contains(y))
                return;
            map[x].Add(y);
        }
        else
            map[x] = new() { y };

        if (x > 0 && wave[x - 1, y].Count > 1 &&
            wave[x - 1, y].RemoveWhere(t => wave[x, y].All(fr => !tiles[t].CanConnectTo(tiles[fr], Side.Bottom))) != 0)
        {
            if (wave[x - 1, y].Count == 0)
                Seted(x - 1, y);
            CountEntropy(x - 1, y, map);
        }
        if (y > 0 && wave[x, y - 1].Count > 1 &&
            wave[x, y - 1].RemoveWhere(t => wave[x, y].All(fr => !tiles[t].CanConnectTo(tiles[fr], Side.Right))) != 0)
        {
            if (wave[x, y - 1].Count == 0)
                Seted(x, y - 1);
            CountEntropy(x, y - 1, map);
        }
        if (x < h - 1 && wave[x + 1, y].Count > 1 &&
            wave[x + 1, y].RemoveWhere(t => wave[x, y].All(fr => !tiles[t].CanConnectTo(tiles[fr], Side.Top))) != 0)
        {
            if (wave[x + 1, y].Count == 0)
                Seted(x + 1, y);
            CountEntropy(x + 1, y, map);
        }
        if (y < w - 1 && wave[x, y + 1].Count > 1 &&
            wave[x, y + 1].RemoveWhere(t => wave[x, y].All(fr => !tiles[t].CanConnectTo(tiles[fr], Side.Left))) != 0)
        {
            if (wave[x, y + 1].Count == 0)
                Seted(x, y + 1);
            CountEntropy(x, y + 1, map);
        }

        map[x].Remove(y);
    }
}
