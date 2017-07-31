using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA.Pathfinding
{
    public class AStarTile
    {
        public AStarTile(float G, float H, AStarTile parent)
        {
            this.G = G;
            this.H = H;
            this.parent = parent;
        }

        public float F
        {
            get
            {
                return G + H;
            }
        }
        public float G;
        public float H;
        public AStarTile parent;
        public int X;
        public int Y;
    }
}
