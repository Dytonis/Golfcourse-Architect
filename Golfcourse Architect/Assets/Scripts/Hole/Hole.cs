using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public Pin currentPin;

    public List<Pin> pinPlacements;
    public List<Tees> TeesList;

    public ChunkFamily family;
    public UIController controller;
    public bool ConstructingCurrently;
    public Dictionary<Tees, List<Vector3>> TargetLine = new Dictionary<Tees, List<Vector3>>();
    public Dictionary<Tees, List<Vector3>> CompressedLine = new Dictionary<Tees, List<Vector3>>();

    //Public<Golfer> golferQueue

    public Dictionary<Tees, float> Distance = new Dictionary<Tees, float>();
    public int Par;
    public bool Valid = false;
    public bool Open = false;

    public void Init(ChunkFamily fam, UIController con)
    {
        family = fam;
        controller = con;
        controller.HoleProperties.Yards.text = " - ";
    }

    public void CalculateTargetLine()
    {
        foreach (Tees t in TeesList)
        {
            GA.Pathfinding.AStarBallFinder finder = new GA.Pathfinding.AStarBallFinder(new Vector2(t.Position.x, t.Position.z), new Vector2(currentPin.PositionFine.x, currentPin.PositionFine.z), family);
            //TargetLine.Add(t, finder.FindPath());
            StartCoroutine(finder.FindPath(FinishedCalculatingTargetLine, t));
        }
    }

    public void CalculateTempLine(Tees t, Pin p)
    {
        Debug.DrawRay(t.Position, Vector3.up * 100, Color.black, 10f);

        GA.Pathfinding.AStarBallFinder finder = new GA.Pathfinding.AStarBallFinder(new Vector2(t.Position.x, t.Position.z), new Vector2(p.PositionFine.x, p.PositionFine.z), family);
        //TargetLine.Add(t, finder.FindPath());
        StartCoroutine(finder.FindPath(FinishedCalculatingTargetLine, t));
    }

    public void CompressLine(Tees t)
    {
        List<Vector3> line;
        List<Vector3> compress = new List<Vector3>();

        if(TargetLine.TryGetValue(t, out line))
        {
            line.Reverse();

            if (line.Count > 0)
            {
                compress.Add(line[0]);

                float distance = 0;
                for (int i = 0; i < line.Count - 1; i++)
                {
                    Vector3 a = line[i];
                    Vector3 b = line[i + 1];
                    distance += Vector3.Distance(new Vector3(a.x, 0, a.z), new Vector3(b.x, 0, b.z));

                    if (i == line.Count - 2)
                    {
                        compress.Add(line[i + 1]);
                    }

                    if (Yard.FloatToYard(distance) >= 250) //put a shootable check here
                    {
                        compress.Add(line[i + 1]);
                        distance = 0;
                    }
                }
            }
        }

        if (CompressedLine.ContainsKey(t))
        {
            CompressedLine[t] = compress;
        }
        else
        {
            CompressedLine.Add(t, compress);
        }
    }

    public void FinishedCalculatingTargetLine(List<Vector3> vectors, Tees t)
    {
        if(vectors.Count == 0)
        {
            controller.HoleProperties.Yards.text = "Unplayable";
            return;
        }

        if (TargetLine.ContainsKey(t))
        {
            TargetLine[t] = vectors;
        }
        else
        {
            TargetLine.Add(t, vectors);
        }

        foreach(Vector3 v in vectors)
        {
            Debug.DrawRay(v, Vector3.up * 100, Color.cyan, 2.5f);
        }

        CompressLine(t);

        t.transform.LookAt(new Vector3(CompressedLine[t][1].x, t.transform.position.y, CompressedLine[t][1].z), Vector3.up);
        t.family = family;
        t.UpdateHeights();

        controller.HoleProperties.Yards.text = CalculateCompressedDistance(t).ToString("#,###");
    }

    private float CalculateDistance(Tees t)
    {
        float distance = 0;
        List<Vector3> vectors = TargetLine[t];

        for (int i = 0; i < vectors.Count - 1; i++)
        {
            Vector3 a = vectors[i];
            Vector3 b = vectors[i + 1];
            distance += Vector3.Distance(new Vector3(a.x, 0, a.z), new Vector3(b.x, 0, b.z));
        }

        return Yard.FloatToYard(distance);
    }

    private float CalculateCompressedDistance(Tees t)
    {
        float distance = 0;
        List<Vector3> vectors = CompressedLine[t];

        for (int i = 0; i < vectors.Count - 1; i++)
        {
            Vector3 a = vectors[i];
            Vector3 b = vectors[i + 1];
            distance += Vector3.Distance(new Vector3(a.x, 0, a.z), new Vector3(b.x, 0, b.z));
        }

        return Yard.FloatToYard(distance);
    }

    public void Update()
    {
        if (ConstructingCurrently)
        {

        }
    }

    public void OnStartConstruction()
    {
        StartCoroutine(controller.MoveRect(controller.SideBarHoleConstructor, new Vector2(-130, -407), new Vector2(200, -407), 5));
        StartCoroutine(controller.MoveRect(controller.TopBarHoleContructor, new Vector2(-41.5f, 9), new Vector2(-41.5f, 150), 5));
    }

    public void OnStopConstruction()
    {
        StartCoroutine(controller.MoveRect(controller.SideBarHoleConstructor, new Vector2(200, -407), new Vector2(-130, -407), 5));
        StartCoroutine(controller.MoveRect(controller.TopBarHoleContructor, new Vector2(-41.5f, 150), new Vector2(-41.5f, 9), 5));
    }

    public void OnLeaveStatsView()
    {
        StartCoroutine(controller.MoveRect(controller.SideBarHoleConstructor, new Vector2(200, -407), new Vector2(-130, -407), 5));
        StartCoroutine(controller.MoveRect(controller.TopBarHoleContructor, new Vector2(-41.5f, 150), new Vector2(-41.5f, 9), 5));
    }

    public void OnViewStats()
    {
        StartCoroutine(controller.MoveRect(controller.SideBarHoleConstructor, new Vector2(-130, -407), new Vector2(200, -407), 5));
        StartCoroutine(controller.MoveRect(controller.TopBarHoleContructor, new Vector2(-41.5f, 9), new Vector2(-41.5f, 150), 5));
    }

    public void OnCreation()
    {
        family.HoleList.Add(this);
    }

    public void OnValidation()
    {

    }

    public void OnOpen()
    {

    }
}
