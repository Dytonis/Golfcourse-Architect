using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GA.Pathfinding;

namespace GA.Pathfinding
{
    public class AStarBallFinder
    {
        public List<AStarTile> openList = new List<AStarTile>();
        public List<AStarTile> closedList = new List<AStarTile>();

        Vector2 globalStart, globalTarget;

        ChunkFamily family;

        public AStarBallFinder(Vector2 start, Vector2 target, ChunkFamily family)
        {
            this.globalStart = start;
            this.globalTarget = target;
            this.family = family;
        }

        public List<Vector3> FindPath()
        {
            AStarTile last = null;
            AStarTile first = new AStarTile(0, Mathf.Abs(globalStart.x - globalTarget.x) + Mathf.Abs(globalStart.y - globalTarget.y), null);
            first.X = (int)globalStart.x;
            first.Y = (int)globalStart.y;
            do
            {
                AStarTile current = FindLowestOpen();

                closedList.Add(current);
                openList.Remove(current);

                if (closedList.Any(x => x.X == (int)globalTarget.x && x.Y == (int)globalTarget.y))
                {
                    last = current;
                    break; //path found
                }

                List<AStarTile> adj = GetAdjacent(new Vector2(current.X, current.Y), current);

                foreach (AStarTile a in adj)
                {
                    if (closedList.Contains(a))
                        continue;
                    if (!openList.Contains(a))
                    {
                        openList.Add(a);
                    }
                    else
                    {
                        //already open
                        if (current.G + a.H < a.F)
                        {
                            a.parent = current;
                        }
                    }
                }

            } while (openList.Count > 0);

            result = new List<Vector3>();
            backupParents(last);
            return result;
        }

        List<Vector3> result = new List<Vector3>();
        private void backupParents(AStarTile current)
        {
            if (current == null)
                return;

            result.Add(new Vector3(current.X, 0, current.Y));
            if (current.parent != null)
            {
                backupParents(current.parent);
            }
        }

        private AStarTile FindLowestOpen()
        {
            AStarTile lowest = null;
            float lowestF = float.PositiveInfinity;
            foreach (AStarTile t in openList)
            {
                if (t.F < lowestF)
                {
                    lowestF = t.F;
                    lowest = t;
                }
            }

            return lowest;
        }

        private List<AStarTile> GetAdjacent(Vector2 pos, AStarTile parent)
        {
            List<AStarTile> tiles = new List<AStarTile>();

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    GA.Ground.GroundType type = family.GetChunkDataPointGroundTypeGlobally((int)pos.x + x, (int)pos.y + y);

                    if (type != null)
                    {
                        if (parent != null)
                        {
                            float G = type.shotWeight + parent.G;
                            float H = Mathf.Abs((pos.x + x) - globalTarget.x) + Mathf.Abs((pos.y + y) - globalTarget.y);

                            AStarTile tile = new AStarTile(G, H, parent);
                            tile.X = (int)(pos.x + x);
                            tile.Y = (int)(pos.y + y);
                            tiles.Add(tile);
                        }
                        else
                        {
                            float G = type.shotWeight;
                            float H = Mathf.Abs((pos.x + x) - globalTarget.x) + Mathf.Abs((pos.y + y) - globalTarget.y);
                            AStarTile tile = new AStarTile(G, H, parent);
                            tile.X = (int)(pos.x + x);
                            tile.Y = (int)(pos.y + y);
                            tiles.Add(tile);
                        }
                    }
                }
            }
            return tiles;
        }
    }
}