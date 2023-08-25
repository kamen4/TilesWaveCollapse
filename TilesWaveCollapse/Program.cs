using TilesWaveCollapse.Generator;
using System.Drawing;
using System.Runtime.Serialization.Formatters;

namespace TilesWaveCollapse;

internal class Program
{
    static void Main(string[] args)
    {
        for (int i = 0; i < 1; i++)
            HOHO(i);
    }

    static void HOHO(int num)
    {
        WfcGenerator g = new(100, 50);
        g.AddTile(new Tile(1, new int[] { 0, 0, 0, 0 }) { img = 0 }, true);
        g.AddTile(new Tile(1, new int[] { 1, 1, 0, 1 }) { img = 3 }, true);
        g.AddTile(new Tile(1, new int[] { 1, 1, 0, 0 }) { img = 2 }, true);
        g.AddTile(new Tile(1, new int[] { 1, 0, 1, 0 }) { img = 1 }, true);
        g.InitField();
        g.Generate();

        Dictionary<int, Dictionary<int, Image>> map = new();
        for (int i = 0; i < g.Tiles.Count; i++)
        {
            int r = g.Tiles[i].Rotation;
            map[i] = new()
            {
                { r, Image.FromFile($"A:\\PROJECTS\\VSNEW\\TilesWaveCollapse\\TilesWaveCollapse\\Images\\s_{g.Tiles[i].img}.png") }
            };
            if (r == 1)
                map[i][r].RotateFlip(RotateFlipType.Rotate270FlipNone);
            else if (r == 2)
                map[i][r].RotateFlip(RotateFlipType.Rotate180FlipNone);
            else if (r == 3)
                map[i][r].RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        Bitmap bmp = new Bitmap(32 * 100, 32 * 50);
        for (int i = 0; i < 50; i++)
            for (int j = 0; j < 100; j++)
                using (var graphics = Graphics.FromImage(bmp))
                {
                    graphics.DrawImage(map[g.Tilemap[i, j].img][g.Tilemap[i, j].Rotation], j * 32, i * 32, 32f, 32f);
                }
        bmp.Save($"C:\\Users\\volde\\Desktop\\folder\\new{num}.png");
    }
}