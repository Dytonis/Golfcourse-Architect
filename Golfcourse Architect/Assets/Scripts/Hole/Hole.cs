using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public string Name;

    public Pin currentPin;

    public List<Pin> pinPlacements;
    public List<Tees> TeesList;

    public Tees lineTeeObject;
    public Pin pinLineObject;

    public ChunkFamily family;
    public UIController controller;
    public bool ConstructingCurrently;
    public Dictionary<Tees, List<Vector3>> TargetLine = new Dictionary<Tees, List<Vector3>>();
    public Dictionary<Tees, List<Vector3>> CompressedLine = new Dictionary<Tees, List<Vector3>>();
    public LineRenderer line;

    //Public<Golfer> golferQueue

    public Dictionary<Tees, float> Distance = new Dictionary<Tees, float>();

    public int Par;
    public bool Valid = false;
    public bool Open = false;

    public void Init(ChunkFamily fam, UIController con)
    {
        family = fam;
        controller = con;
        Name = "Hole #" + (fam.HoleList.Count + 1).ToString();
        controller.HoleProperties.Yards.text = " - ";
        controller.HoleProperties.TopTitle.text = Name + " Under Construction";
        controller.HoleProperties.Title.text = Name;
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

    GA.Pathfinding.AStarBallFinder last;
    public void CalculateTempLine(Tees t, Pin p)
    {
        Debug.DrawRay(t.Position, Vector3.up * 100, Color.black, 10f);

        if (last != null)
            last.Abort = true;

        GA.Pathfinding.AStarBallFinder finder = new GA.Pathfinding.AStarBallFinder(new Vector2(t.Position.x, t.Position.z), new Vector2(p.PositionFine.x, p.PositionFine.z), family);
        //TargetLine.Add(t, finder.FindPath());
        StartCoroutine(finder.FindPath(FinishedCalculatingTargetLine, t));
        last = finder;
    }

    public void CompressLine(Tees t)
    {
        List<Vector3> line;
        List<Vector3> compress = new List<Vector3>();

        if (TargetLine.TryGetValue(t, out line))
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
                        compress.Add(new Vector3(line[i + 1].x + 0.5f, line[i + 1].y, line[i + 1].z + 0.5f));
                    }

                    if (Yard.FloatToYard(distance) >= 50) //put a shootable check here
                    {
                        compress.Add(new Vector3(line[i + 1].x + 0.5f, line[i + 1].y, line[i + 1].z + 0.5f));
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
        if (vectors.Count == 0)
        {
            controller.HoleProperties.Yards.text = "Unplayable";
            controller.HoleProperties.Par.text = "1";
            line.enabled = false;
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

        foreach (Vector3 v in vectors)
        {
            Debug.DrawRay(v, Vector3.up * 100, Color.cyan, 2.5f);
        }

        CompressLine(t);

        if (CompressedLine[t].Count <= 0)
        {
            return;
        }

        if (t)
        {
            t.transform.LookAt(new Vector3(CompressedLine[t][1].x, t.transform.position.y, CompressedLine[t][1].z), Vector3.up);
            t.Fencing.transform.localRotation = Quaternion.Euler(0, -t.transform.localRotation.eulerAngles.y, 0);
            t.family = family;
            t.UpdateHeights();

            if (Distance.ContainsKey(t))
                Distance[t] = CalculateCompressedDistance(t);
            else
                Distance.Add(t, CalculateCompressedDistance(t));


            Distance = Distance.Where(item => item.Key != null).ToDictionary(i => i.Key, i => i.Value);

            controller.HoleProperties.Yards.text = Distance[t].ToString("#,###");
            Par = GetParFromDistance(Distance[t]);
            controller.HoleProperties.Par.text = Par.ToString();

            if (line)
                line.enabled = true;
        }

        if (line && lineTeeObject && pinLineObject)
        {
            if (CompressedLine.ContainsKey(lineTeeObject))
            {
                List<Vector3> vs = new List<Vector3>();

                foreach (Vector3 v in CompressedLine[lineTeeObject])
                {
                    vs.Add(new Vector3(v.x, family.GetElevationUnderPointGlobalRaycast(v.x, v.z) + 0.25f, v.z));
                }

                vs[vs.Count - 1] = new Vector3(pinLineObject.transform.position.x,
                    family.GetElevationUnderPointGlobalRaycast(pinLineObject.transform.position.x, pinLineObject.transform.position.z) + 0.25f,
                    pinLineObject.transform.position.z);

                line.positionCount = vs.Count;
                line.SetPositions(vs.ToArray());
            }
        }
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

    public float GetShortestTee()
    {
        float shortest = float.PositiveInfinity;

        foreach (Tees t in TeesList)
        {
            if (Distance.ContainsKey(t))
            {
                if (shortest > Distance[t])
                    shortest = Distance[t];
            }
        }

        return shortest;
    }

    public float GetLongestTee()
    {
        float longest = 0;

        foreach (Tees t in TeesList)
        {
            if (Distance.ContainsKey(t))
            {
                if (longest < Distance[t])
                    longest = Distance[t];
            }
        }

        return longest;
    }

    public void Update()
    {
        if (ConstructingCurrently)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (pinPlacements.Count > 0 && TeesList.Count > 0)
                {
                    OnOpen();
                }
            }
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
        controller.TeeButton.EnableButton();
        controller.PinButton.EnableButton();
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
        Open = true;
        ConstructingCurrently = false;
        OnStopConstruction();

        family.CurrentHoleCreating = null;

        if (Distance.Count > 0)
        {
            if (Distance.Count >= 2)
                controller.MessageBar.QueuePopMessage(Name + ", a par " + Par + " between " + GetShortestTee().ToString("#,###") + " and " + GetLongestTee().ToString("#,###") + " yards, is now open for play.", 3);
            else
                controller.MessageBar.QueuePopMessage(Name + ", a " + GetLongestTee().ToString("#,###") + " yard par " + Par + ", is now open for play.", 2);
        }
        foreach (Tees t in TeesList)
        {
            t.RemoveFencing();
        }
    }

    public int GetParFromDistance(float distance)
    {
        if (distance <= 50)
            return 2;
        else if (distance < 250)
            return 3;
        else if (distance < 420)
            return 4;
        else if (distance < 610)
            return 5;
        else return 6;
    }

    public LineRenderer GetDrawLine()
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = Vector3.zero;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = Color.white;
        lr.endColor = Color.white;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        return lr;
    }
}
