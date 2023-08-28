using System;
using System.Drawing.Drawing2D;
using System.Reflection.Metadata.Ecma335;
using TilesWaveCollapse.Generator;

namespace TileIRT
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Dictionary<int, Dictionary<int, Image>> map;
        WfcGenerator g;
        int w = 68, h = 36;
        int tileScale = 28;
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
            bool rots = true;
            g.AddTile(new Tile(1, new int[] { 0, 0, 0, 0 }) { img = 0 }, rots);
            g.AddTile(new Tile(1, new int[] { 1, 1, 0, 1 }) { img = 3 }, rots);
            g.AddTile(new Tile(1, new int[] { 1, 1, 0, 0 }) { img = 2 }, rots);
            g.AddTile(new Tile(1, new int[] { 1, 0, 1, 0 }) { img = 1 }, rots);
            //g.AddTile(new Tile(1, new int[] { 1, 1, 1, 1 }) { img = 4 }, rots);
            //g.AddTile(new Tile(1, new int[] { 1, 0, 0, 0 }) { img = 5 }, rots);

            bmp = new Bitmap(tileScale * w, tileScale * h);
            pictureBox1.Image = bmp;
            Update();
            this.Size = bmp.Size + new Size(20, 40);
            this.CenterToScreen();
            pictureBox1.Size = bmp.Size;

            map = new();
            for (int i = 0; i < g.Tiles.Count; i++)
            {
                int r = g.Tiles[i].Rotation;
                int img = g.Tiles[i].img;
                if (!map.ContainsKey(img))
                    map[img] = new();
                map[img].Add(r, Image.FromFile($"A:\\PROJECTS\\VSNEW\\TilesWaveCollapse\\TilesWaveCollapse\\Images\\s_{g.Tiles[i].img}.png"));
                if (r == 1)
                    map[img][r].RotateFlip(RotateFlipType.Rotate270FlipNone);
                else if (r == 2)
                    map[img][r].RotateFlip(RotateFlipType.Rotate180FlipNone);
                else if (r == 3)
                    map[img][r].RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            g.InitField();
            g.SetBordersEmpty();
            g.Generate();
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    using (var graphics = Graphics.FromImage(bmp))
                    {
                        var tile = g.Tilemap[i, j];
                        graphics.DrawImage(map[tile.img][tile.Rotation], j * tileScale, i * tileScale, tileScale, tileScale);
                    }
            pictureBox1.Image = bmp;
            Update();
            var list = PathFinder.PathFinder.FindPath(g.MAZE());
            if (list == null)
            {
                button1_Click(sender, e);
                return;
            }
            using (var b = new SolidBrush(Color.Red))
            using (var graphics = Graphics.FromImage(bmp))
            {
                foreach (var pair in list)
                {
                    graphics.FillRectangle(b, pair.Item2 * tileScale / 3f, pair.Item1 * tileScale / 3f, tileScale / 3f, tileScale / 3f);
                    pictureBox1.Image = bmp;
                    Update();
                }
            }
            button1.Visible = true;
        }
    }
}