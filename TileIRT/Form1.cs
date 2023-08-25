using System;
using TilesWaveCollapse.Generator;

namespace TileIRT
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Dictionary<int, Image> map;
        WfcGenerator g;
        int w = 200, h = 100;
        int tileScale = 8;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        int Setted(int i, int j)
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawImage(map[g.wave[i, j].DefaultIfEmpty(0).First()], j * tileScale, i * tileScale, tileScale, tileScale);
            }
            pictureBox1.Image = bmp;
            Update();
            return 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            g = new(w, h);
            g.Seted = Setted;
            g.AddTile(new Tile(1, new int[] { 0, 0, 0, 0 }) { img = 0 }, true);
            g.AddTile(new Tile(1, new int[] { 1, 1, 0, 1 }) { img = 3 }, true);
            g.AddTile(new Tile(1, new int[] { 1, 1, 0, 0 }) { img = 2 }, true);
            g.AddTile(new Tile(1, new int[] { 1, 0, 1, 0 }) { img = 1 }, true);

            bmp = new Bitmap(tileScale * w, tileScale * h);
            this.Size = bmp.Size + new Size(16, 30);
            pictureBox1.Size = bmp.Size;

            map = new();
            for (int i = 0; i < g.tiles.Count; i++)
            {
                map.Add(i, Image.FromFile($"A:\\PROJECTS\\VSNEW\\TilesWaveCollapse\\TilesWaveCollapse\\Images\\s_{g.tiles[i].img}.png"));
                if (g.tiles[i].Rotation == 1)
                    map[i].RotateFlip(RotateFlipType.Rotate270FlipNone);
                else if (g.tiles[i].Rotation == 2)
                    map[i].RotateFlip(RotateFlipType.Rotate180FlipNone);
                else if (g.tiles[i].Rotation == 3)
                    map[i].RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            g.Generate();

            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    using (var graphics = Graphics.FromImage(bmp))
                    {
                        graphics.DrawImage(map[g.wave[i, j].DefaultIfEmpty(0).First()], j * tileScale, i * tileScale, tileScale, tileScale);
                    }
            pictureBox1.Image = bmp;
        }
    }
}