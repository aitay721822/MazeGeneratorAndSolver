using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace 迷宮產生與解題
{
    class Node
    {
        public Point _pos { get; set; }
        private bool _wall { get; set; }

        public Node(int x,int y)
        {
            _pos = new Point(x, y);
            _wall = false;
        }

        public int Euclidean(Node goal)
        {
            double distance = Math.Sqrt(Math.Pow((_pos.X - goal._pos.X), 2) + Math.Pow((_pos.Y - goal._pos.Y), 2));
            return Convert.ToInt32(distance);
        }

        public int Manhattan(Node goal)
        {
            int distance = Math.Abs(_pos.X - goal._pos.X) + Math.Abs(_pos.Y - goal._pos.Y);
            return distance;
        }

        public void SetWall(bool value)
        {
            _wall = value;
        }

        public bool _isWall()
        {
            return _wall;
        }
    }
}
