namespace PathFinder
{
    public static class PathFinder
    {
        public static List<(int, int)>? FindPath(int[,] grid)
        {
            int h = grid.GetLength(0);
            int w = grid.GetLength(1);

            (int, int)[] dirs = new (int, int)[] { (1, 0), (0, 1), (-1, 0), (0, -1) };

            PriorityQueue<(int, int, int), int> pq = new();
            pq.Enqueue((h - 3, w - 2, 0), 0);
            while (pq.Count != 0)
            {
                var cur = pq.Dequeue();
                int i = cur.Item1;
                int j = cur.Item2;
                int dist = cur.Item3;
                if (Math.Min(i, j) < 1 || i > h || j > w || grid[i, j] <= dist)
                {
                    continue;
                }

                grid[i, j] = dist;
                if (i == 1 && j == 2)
                {
                    break;
                }

                foreach (var d in dirs)
                {
                    int x = i + d.Item1;
                    int y = j + d.Item2;
                    pq.Enqueue((x, y, dist + 1), (dist + 1));
                }
            }

            if (grid[1, 2] == int.MaxValue)
            {
                return null;
            }

            grid[h - 3, w - 2] = 0;
            List<(int, int)> l = new();
            int numb = grid[1, 2];
            int r = 1, c = 2;
            l.Add((r - 1, c - 1));
            while (numb != -1)
            {
                foreach (var d in dirs)
                {
                    if (grid[r + d.Item1, c + d.Item2] == numb)
                    {
                        r += d.Item1;
                        c += d.Item2;
                        l.Add((r - 1, c - 1));
                        break;
                    }
                }

                --numb;
            }

            return l;
        }
    }
}