using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace 迷宮產生與解題
{
    class MazeGenerator
    {
        private static Random r = new Random();
        private int Width { get; set; }
        private int Height { get; set; }
        public Cell[,] Maze { get; set; }
        private Point[] Shift = new Point[] { new Point(0, -1), new Point(0, 1), new Point(-1, 0), new Point(1, 0) };
        private string[] Direction = new string[] {"top","bot","left","right" };
        private List<Cell> unVisited;
        private Stopwatch sw = new Stopwatch();
        /// <summary>
        /// 初始化MazeGenerator
        /// </summary>
        /// <param name="width">寬度</param>
        /// <param name="height">高度</param>
        public MazeGenerator(int width,int height)
        {
            this.Width = width;
            this.Height = height;
            this.Maze = new Cell[width, height];
            this.unVisited = new List<Cell>();

            for (int i = 0; i < Maze.GetLength(0); i++)
                for (int j = 0; j < Maze.GetLength(1); j++)
                {
                    Maze[i, j] = new Cell(new Point(i, j));
                    unVisited.Add(Maze[i, j]);
                }
        }
        /// <summary>
        /// 取得result
        /// </summary>
        /// <param name="squre">方形大小</param>
        /// <returns></returns>
        public Bitmap _result(int squre)
        {
            Bitmap _r = new Bitmap(Width * squre + 1, Height * squre + 1);
            Graphics g = Graphics.FromImage(_r);
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, _r.Width, _r.Height));
            if (Maze != null)
            {
                for (int x = 0; x < Maze.GetLength(0); x++)
                    for (int y = 0; y < Maze.GetLength(1); y++)
                    {
                        DrawRect(g, new Pen(Color.Black), Maze[x, y]._wall, squre, squre, x * squre, y * squre);
                    }
            }
            return _r;
        }

        /// <summary>
        /// Draw Bounding Box
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="pen">Pen(color)</param>
        /// <param name="side">Wall</param>
        /// <param name="height">Squre Height</param>
        /// <param name="width">Squre Width</param>
        /// <param name="x">start X</param>
        /// <param name="y">start Y</param>
        public void DrawRect(Graphics g,Pen pen,int[] side,int height,int width,int x,int y)
        {
            //上下左右
            for(int i = 0; i < side.Length; i++)
                if (side[i] == 1)
                {
                    switch (i)
                    {
                        case 0:
                            g.DrawLine(pen, x, y, width + x, y);
                            break;
                        case 1:
                            g.DrawLine(pen, x, height + y, width + x, height + y);
                            break;
                        case 2:
                            g.DrawLine(pen, x, y, x, height + y);
                            break;
                        case 3:
                            g.DrawLine(pen, width + x, y, width + x, height + y);
                            break;
                    }
                }
        }

        public bool AllVisited()
        {
            if (unVisited.Count > 0) return false;
            else return true;
        }

        Stack<Cell> stack = new Stack<Cell>();
        public void Generator()
        {
            //策略：先找尋起點
            Point stPoint = new Point(r.Next(Maze.GetLength(0)), 0); //從0 ~ width取數字，且將y軸指定到最上方
            //將起點的上方開洞開始走訪
            Cell Initial = Maze[stPoint.X, stPoint.Y];
            Initial._Position = new Point(stPoint.X, stPoint.Y);
            Initial._unWall(1);
            Initial._visited = true;

            stack.Push(Initial); //推入堆疊

            //sw.Reset();
            //sw.Start();
            while (stack.Count > 0)
            {
                Cell current = stack.Peek();

                List<Cell> neighbor = getNeighbor(current._Position);
                if (AllVisited()) break;
                if (neighbor.Count == 0)
                {
                    stack.Pop();
                    current._visited = true;
                    unVisited.Remove(current);
                    Maze[current._Position.X, current._Position.Y] = current;
                    continue;
                }

                Cell randomNeighbor = neighbor[r.Next(0, 1000) % neighbor.Count];  //隨機選定一個neighbor

                neighbor.Remove(randomNeighbor);

                for (int i = 0; i < Shift.Length; i++)
                {
                    if (randomNeighbor._Position.X == current._Position.X + Shift[i].X &&
                        randomNeighbor._Position.Y == current._Position.Y + Shift[i].Y)
                    {
                        string getDirection = Direction[i]; //取得往下or往上or往左or往右
                        switch (getDirection) //拆牆壁!
                        {
                            case "top":
                                current._unWall(1);
                                randomNeighbor._unWall(2);
                                break;
                            case "bot":
                                current._unWall(2);
                                randomNeighbor._unWall(1);
                                break;
                            case "left":
                                current._unWall(3);
                                randomNeighbor._unWall(4);
                                break;
                            case "right":
                                current._unWall(4);
                                randomNeighbor._unWall(3);
                                break;
                        }
                    }
                }

                //將此點設為已探索
                current._visited = true;
                unVisited.Remove(current);
                Maze[current._Position.X, current._Position.Y] = current;

                stack.Push(randomNeighbor);
            }
            //sw.Stop();
            //MessageBox.Show(sw.ElapsedMilliseconds.ToString());

            Point enPoint = new Point(r.Next(Maze.GetLength(0)), Maze.GetLength(1) - 1); //從0 ~ width取數字，且將y軸指定到最下方
            Cell EndPoint = Maze[enPoint.X, enPoint.Y];
            EndPoint._unWall(2);
            Maze[enPoint.X, enPoint.Y] = EndPoint;
        }


        /* 遞歸方法
        /// <summary>
        /// 產生迷宮
        /// </summary>
        public void Generator()
        {
            
            //策略：先找尋起點
            Point stPoint = new Point(r.Next(Maze.GetLength(0)), 0); //從0 ~ width取數字，且將y軸指定到最上方
            //將起點的上方開洞開始走訪
            Cell Initial = Maze[stPoint.X, stPoint.Y];
            Initial._Position = new Point(stPoint.X, stPoint.Y);
            Initial._unWall(1);
            Initial._visited = true;

            Travel(Initial);

            Point enPoint = new Point(r.Next(Maze.GetLength(0)), Maze.GetLength(1) - 1); //從0 ~ width取數字，且將y軸指定到最下方
            Cell EndPoint = Maze[enPoint.X, enPoint.Y];
            EndPoint._unWall(2);
            Maze[enPoint.X, enPoint.Y] = EndPoint;
        }

        
        public void Travel(Cell current)
        {
            List<Cell> neighbor = getNeighbor(current._Position); //取得鄰居
            while (!AllVisited())
            {
                neighbor = getNeighbor(current._Position);
                if (neighbor.Count == 0)
                {
                    current._visited = true;
                    unVisited.Remove(current);
                    Maze[current._Position.X, current._Position.Y] = current;
                    break;
                }

                Cell randomNeighbor = neighbor[r.Next(0, 1000) % neighbor.Count];  //隨機選定一個neighbor

                neighbor.Remove(randomNeighbor);

                for(int i =0; i < Shift.Length; i++)
                {
                    if (randomNeighbor._Position.X == current._Position.X + Shift[i].X &&
                        randomNeighbor._Position.Y == current._Position.Y + Shift[i].Y)
                    {
                        string getDirection = Direction[i]; //取得往下or往上or往左or往右
                        switch (getDirection) //拆牆壁!
                        {
                            case "top":
                                current._unWall(1);
                                randomNeighbor._unWall(2);
                                break;
                            case "bot":
                                current._unWall(2);
                                randomNeighbor._unWall(1);
                                break;
                            case "left":
                                current._unWall(3);
                                randomNeighbor._unWall(4);
                                break;
                            case "right":
                                current._unWall(4);
                                randomNeighbor._unWall(3);
                                break;
                        }
                    }
                }

                //將此點設為已探索
                current._visited = true;
                unVisited.Remove(current);
                Maze[current._Position.X, current._Position.Y] = current;
                Travel(randomNeighbor); //繼續探訪  
            }
        }
        */



        /// <summary>
        /// 取得目前節點的鄰居
        /// </summary>
        /// <param name="nowPos">現在位置</param>
        /// <returns></returns>
        public List<Cell> getNeighbor(Point nowPos)
        {
            List<Cell> _neighbor = new List<Cell>();
            int thisX = nowPos.X;
            int thisY = nowPos.Y;

            for(int i = 0; i < Shift.Length; i++)
            {
                if(thisX + Shift[i].X >= 0 && thisX + Shift[i].X < Maze.GetLength(0)&&
                   thisY + Shift[i].Y >= 0 && thisY + Shift[i].Y < Maze.GetLength(1))
                {
                    Cell current = Maze[thisX + Shift[i].X, thisY + Shift[i].Y];
                    if (!current._visited)
                    {
                        _neighbor.Add(current);
                    }
                } 
            }
            return _neighbor;
        }

        public void Save(string filename)
        {
            _result(5).Save(filename);
        }
    }
}
