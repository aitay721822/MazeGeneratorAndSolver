using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace 迷宮產生與解題
{
    class MazeSolver
    {
        private Node[,] Maze;
        public MazeSolver(Bitmap bitmap,int bpthreshold)
        {
            Maze = new Node[bitmap.Width, bitmap.Height];

            BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                var getData = bd.Scan0;
                int _str = bd.Stride;
                int _wbyte = bd.Width * 3; //rgb
                int _skip = _str - _wbyte;

                byte* p = (byte*)(getData);
                for (int j = 0; j < bitmap.Height; j++)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        Maze[i, j] = new Node(i, j); //Set Position

                        int B = p[0];
                        p++;
                        int G = p[0];
                        p++;
                        int R = p[0];
                        p++;

                        int gray = (B + G + R) / 3; //灰階化

                        if (gray >= bpthreshold) { Maze[i, j].SetWall(false); }
                        else Maze[i, j].SetWall(true);
                   }
                    p += _skip;
                }
                bitmap.UnlockBits(bd);
            }
        }


        public Stack<Node> Path = new Stack<Node>();

        private Point[] Shift = new Point[] {
            new Point(0, -1), new Point(-1, 0), new Point(1, 0), new Point(0, 1)
        };

        /// <summary>
        /// IDA star algorithm
        /// </summary>
        public Stack<Node> Solver()
        {
            if (Maze != null)
            {
                Point startPoint = new Point();
                Point endPoint = new Point();
                for (int i = 0; i < Maze.GetLength(0); i++)
                {
                    if (!Maze[i, 0]._isWall()) startPoint = new Point(i, 0);
                    if (!Maze[i, Maze.GetLength(1) - 1]._isWall()) endPoint = new Point(i, Maze.GetLength(1) - 1);
                    if (!startPoint.IsEmpty && !endPoint.IsEmpty) break;
                }
                Solve(startPoint, endPoint);
                return Path;
            }
            else return null;
        }

        /// <summary>
        /// IDA star algorithm
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="End"></param>
        private void Solve(Point current, Point goal)
        {
            Node initial = Maze[current.X, current.Y];
            Node goals = Maze[goal.X, goal.Y];
            int thresh = initial.Euclidean(goals);
            int Steps = 0;
            bool found = false;
            Path.Push(Maze[current.X, current.Y]);
            while (!found && Steps < 3000)
            {
                thresh = Search(initial, goals, 0, thresh, ref found);
                Steps++;
            }
        }

        private int Search(Node current,Node goal,int gx,int thresold,ref bool found)
        {
            int hx = current.Euclidean(goal);
            if (gx + hx > thresold) return gx + hx;
            if (hx == 0) { found = true; return gx; }

            int next_bound = int.MaxValue;
            List<Node> Corner = _getAround(current);
            for(int i = 0; i < Corner.Count; i++)
            {
                Node cell = Corner[i];
                Path.Push(cell);
                int temp = Search(cell, goal, gx + 1, thresold, ref found);
                if (found) return temp;
                if (temp < next_bound) next_bound = temp;
                Path.Pop();
            }
            return next_bound;
        }

        private List<Node> _getAround(Node current)
        {
            List<Node> result = new List<Node>();
            for(int x = 0; x < Shift.Length; x++)
                if (current._pos.X + Shift[x].X >= 0 && current._pos.X + Shift[x].X < Maze.GetLength(0) &&
                    current._pos.Y + Shift[x].Y >= 0 && current._pos.Y + Shift[x].Y < Maze.GetLength(1))
                {
                    Node neighbor = Maze[current._pos.X + Shift[x].X, current._pos.Y + Shift[x].Y];
                    if (!neighbor._isWall()&&!Path.Contains(neighbor)) //如果鄰居不是牆
                    {
                        result.Add(neighbor);
                    }
                }
            return result;
        }
    }
}
