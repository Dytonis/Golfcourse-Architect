using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GA.Pathfinding;
using System;

namespace GA.Pathfinding
{
    public class AStarGolferFinder
    {
        public List<AStarTile> openList = new List<AStarTile>();
        public List<AStarTile> closedList = new List<AStarTile>();

        public Vector2 globalStart, globalTarget;

        public bool Abort = false;

        ChunkFamily family;

        public DebugHelper helper;

        public AStarGolferFinder(Vector2 start, Vector2 target, ChunkFamily family)
        {
            this.globalStart = start;
            this.globalTarget = target;
            this.family = family;
        }

        public IEnumerator FindPath(Action<Vector2[]> callback)
        {
            AStarTile last = null;
            AStarTile first = new AStarTile(0, getDistance(globalStart, globalTarget), null);
            openList.Add(first);
            first.X = (int)globalStart.x;
            first.Y = (int)globalStart.y;
            int index = 0;
            do
            {
                AStarTile current = FindLowestOpen();

                if (Abort)
                    yield break;

                if(index % 50 == 0)
                yield return new WaitForEndOfFrame();

                if (openList.Count > 0)
                {
                    closedList.Add(current);
                    openList.Remove(current);
                }

                if (closedList.Any(x => x.X == (int)globalTarget.x && x.Y == (int)globalTarget.y))
                {
                    last = current;
                    break; //path found
                }

                List<AStarTile> adj = GetAdjacent(new Vector2(current.X, current.Y), current);

                foreach (AStarTile a in adj)
                {

                    if (containsInClosed(a.X, a.Y))
                        continue;

                    GA.Ground.GroundType type = family.GetChunkDataPointGroundTypeGlobally(a.X, a.Y);

                    if (!type.walkable)
                        continue;

                    float adjMoveCost = current.G + getDistance(new Vector2(current.X, current.Y), new Vector2(a.X, a.Y)) + type.walkWeight;

                    if(adjMoveCost < a.G || !containsInOpen(a.X, a.Y))
                    {
                        a.G = adjMoveCost;
                        a.H = getDistance(new Vector2(a.X, a.Y), globalTarget);
                        a.parent = current;

                        if (!containsInOpen(a.X, a.Y))
                            openList.Add(a);
                    }
                }
                index++;
            } while (openList.Count > 0 && index < 666);

            result = new List<Vector2>();
            backupParents(last);
            result.Reverse();
            callback(result.ToArray());
            yield break;
        }

        List<Vector2> result = new List<Vector2>();
        private void backupParents(AStarTile current)
        {
            if (current == null)
                return;

            Debug.DrawRay(new Vector3(current.X, 0, current.Y), Vector3.up * 100, Color.red, 10f);
            result.Add(new Vector3(current.X, current.Y));
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

        private bool containsInOpen(int X, int Y)
        {
            foreach (AStarTile t in openList)
            {
                if (t.X == X && t.Y == Y)
                    return true;
            }

            return false;
        }

        private bool containsInClosed(int X, int Y)
        {
            foreach (AStarTile t in closedList)
            {
                if (t.X == X && t.Y == Y)
                    return true;
            }

            return false;
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

                    AStarTile tile = new AStarTile(-1, -1, parent);
                    tile.X = (int)(pos.x + x);
                    tile.Y = (int)(pos.y + y);
                    tiles.Add(tile);
                }
            }
            return tiles;
        }

        private float getDistance(Vector2 a, Vector2 b)
        {
            int horizontal = (int)Mathf.Abs(a.x - b.x);
            int vertical = (int)Mathf.Abs(a.y - b.y);

            if (horizontal > vertical)
            {
                return (1.4f * vertical) + (horizontal - vertical);
            }
            else
            {
                return (1.4f * horizontal) + (vertical - horizontal);
            }
        }
    }
}