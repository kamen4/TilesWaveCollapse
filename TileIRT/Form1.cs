using System;
using System.Reflection.Metadata.Ecma335;
using TilesWaveCollapse.Generator;

namespace TileIRT
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Dictionary<int, Dictionary<int, Image>> map;
        WfcGenerator g;
        int w = 2, h = 2;
        int tileScale = 32;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        void Setted(int i, int j)
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var tile = g.Tilemap[i, j];
                graphics.DrawImage(map[tile.img][tile.Rotation], j * tileScale, i * tileScale, tileScale, tileScale);
            }
            pictureBox1.Image = bmp;
            Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            g = new(w, h);
            g.TileSetted = Setted;
            g.AddTile(new Tile(1, new int[] { 0, 0, 0, 0 }) { img = 0 }, true);
            g.AddTile(new Tile(1, new int[] { 1, 1, 0, 1 }) { img = 3 }, true);
            g.AddTile(new Tile(1, new int[] { 1, 1, 0, 0 }) { img = 2 }, true);
            g.AddTile(new Tile(1, new int[] { 1, 0, 1, 0 }) { img = 1 }, true);

            bmp = new Bitmap(tileScale * w, tileScale * h);
            this.Size = bmp.Size + new Size(16, 30);
            pictureBox1.Size = bmp.Size;

            map = new();
            for (int i = 0; i < g.Tiles.Count; i++)
            {
                int r = g.Tiles[i].Rotation;
                int img = g.Tiles[i].img;
                if (!map.ContainsKey(img))
                    map[img] = new();
                map[img].Add(r, Image.FromFile($"A:\\PROJECTS\\VSNEW\\TilesWaveCollapse\\TilesWaveCollapse\\Images\\f_{g.Tiles[i].img}.png"));
                if (r == 1)
                    map[img][r].RotateFlip(RotateFlipType.Rotate270FlipNone);
                else if (r == 2)
                    map[img][r].RotateFlip(RotateFlipType.Rotate180FlipNone);
                else if (r == 3)
                    map[img][r].RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            g.InitField();
            g.Generate();

            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    using (var graphics = Graphics.FromImage(bmp))
                    {
                        var tile = g.Tilemap[i, j];
                        graphics.DrawImage(map[tile.img][tile.Rotation], j * tileScale, i * tileScale, tileScale, tileScale);
                    }
            pictureBox1.Image = bmp;
        }
    }
}