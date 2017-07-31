using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public Pin currentPin;
    public Tees currentTees;

    public List<Vector3> pinPlacements;
    public List<Tees> TeesList;

    public ChunkFamily family;
    public Dictionary<Tees, List<Vector3>> TargetLine = new Dictionary<Tees, List<Vector3>>();

    //Public<Golfer> golferQueue

    public float Distance;
    public int Par;

    public void CalculateTargetLine()
    {
        foreach (Tees t in TeesList)
        {
            GA.Pathfinding.AStarBallFinder finder = new GA.Pathfinding.AStarBallFinder(t.Position, currentPin.PositionFine, family);
            TargetLine.Add(t, finder.FindPath());

            foreach(List<Vector3> l in TargetLine.Values)
            {
                foreach(Vector3 v in l)
                {
                    Debug.DrawRay(v, Vector3.up, Color.black, 10f);
                }
            }
        }
    }
}
