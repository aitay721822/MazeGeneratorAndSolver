using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace 迷宮產生與解題
{
    class Cell :IDisposable
    {
        public Point _Position { get; set; }
        public bool _visited { get; set; }
        public int[] _wall = { 1, 1, 1, 1 }; //first bit = top, sec bit = bottom, third bit = left, four bit = right

        public Cell(Point nowPos)
        {
            _Position = nowPos;
            _visited = false;
            _wall = new int[4] { 1, 1, 1, 1 };
        }

        public void Dispose()
        {

        }

        /// <summary>
        /// RemoveWall
        /// </summary>
        /// <param name="index">1 is top, 2 is bottom, 3 is left, 4 is right</param>
        public void _unWall(int index)
        {
            _wall[(index - 1) % _wall.Length] = 0;
        }

        /// <summary>
        /// InstallWall
        /// </summary>
        /// <param name="index">1 is top, 2 is bottom, 3 is left, 4 is right</param>
        public void _InstallWall(int index)
        {
            _wall[(index - 1) % _wall.Length] = 1;
        }

        /// <summary>
        /// Fill Wall
        /// </summary>
        public void FillWall()
        {
            _wall = new int[4] { 1, 1, 1, 1 };
        }

        /// <summary>
        /// Clear Wall
        /// </summary>
        public void ClearWall()
        {
            _wall = new int[4] { 0, 0, 0, 0 };
        }
    }
}
