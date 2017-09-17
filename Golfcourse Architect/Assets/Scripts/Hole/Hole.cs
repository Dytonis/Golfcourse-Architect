using GA;
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

    public StandardGamemode gamemode;
    public ChunkFamily family;
    public UIController controller;
    public bool ConstructingCurrently;
    public Dictionary<Tees, List<Vector3>> TargetLine = new Dictionary<Tees, List<Vector3>>();
    public Dictionary<Tees, List<Vector3>> CompressedLine = new Dictionary<Tees, List<Vector3>>();
    public LineRenderer line;
    public Material lineMaterial;

    //Public<Golfer> golferQueue

    public Dictionary<Tees, float> Distance = new Dictionary<Tees, float>();

    public int Par;
    public bool Valid = false;
    public bool Open = false;

    public void Init(StandardGamemode game, ChunkFamily fam, UIController con)
    {
        family = fam;
        gamemode = game;
        controller = con;
        Name = "Hole #" + (gamemode.HoleList.Count + 1).ToString();
        controller.HoleProperties.Yards.text = " - ";
        controller.HoleProperties.TopTitle.text = Name + " Under Construction";
        controller.HoleProperties.Title.text = Name;
    }

    [System.Obsolete]
    public ShotPath Golfer_CalculateShotPosition(Golfer g, Vector2 fromPosition)
    {
        if (CompressedLine.ContainsKey(g.gamemode.getTeebox(g.round.TeeType, g.round.CurrentHole)))
        {
            List<Vector3> list = CompressedLine[g.gamemode.getTeebox(g.round.TeeType, g.round.CurrentHole)];

            if (list.Count > 8)
                list.RemoveRange(7, list.Count - 8);

            List<List<Vector3>> possiblePaths = new List<List<Vector3>>();

            for (byte i = 0; i < Mathf.Pow((list.Count - 2), 2); i++)
            {
                List<Vector3> path = new List<Vector3>();
                path.Add(new Vector3(fromPosition.x, 0, fromPosition.y));

                for (byte y = 0; y < list.Count - 2; y++)
                {
                    if (IsBitSet(i, y))
                    {
                        path.Add(list[y + 1]);
                    }
                }

                path.Add(list[list.Count - 1]); //add the first and last positions

                possiblePaths.Add(path);
            }

            ShotPath reference = new ShotPath();
            reference.yList = new List<float>();
            reference.shotCount = list.Count;
            reference.points = listVector3ToVector2(list);
            for (int i = 0; i < list.Count - 1; i++)
            {
                reference.r += getRiskBetweenPoints(list[i], list[i + 1]); //get risk between the first list's current point and next
                //reference.yList.Add(Yard.FloatToYard(Vector2.Distance(list[i], list[i + 1])));
                //reference.y += reference.yList[i];
            }
            List<ShotPath> paths = new List<ShotPath>();
            //we have all the possible paths
            foreach (List<Vector3> path in possiblePaths)
            {
                ShotPath p = new ShotPath();
                bool valid = true;
                for (int i = 0; i < path.Count - 1; i++)
                {
                    if (Yard.FloatToYard(Vector2.Distance(path[i], path[i + 1])) > g.Stats.DriverLength)
                    {
                        valid = false;
                        break;
                    }

                    p.r += getRiskBetweenPoints(path[i], path[i + 1]); //get risk between the current point and the next
                    //p.y += (Yard.FloatToYard(Vector2.Distance(path[i], path[i + 1])) - reference.yList[i]); //calculate difference in yardage between all points path and current path points

                    Debug.DrawRay(path[i], Vector3.up * 100, Color.cyan, 10f);
                }

                if (!valid) //if a shot is too far, ignore the path as it is impossible
                    continue;

                p.shotCount = path.Count;
                p.points = listVector3ToVector2(path);

                paths.Add(p);
            }

            //calculate preferred path
            //get the path with the shortest distance with the lowst p score under MSC

            ShotPath preferred = getLowestPriceShot(paths);

            foreach (ShotPath p in paths)
            {
                if (p.p > g.Personality.MaxShotCost)
                    continue;

                if (Vector2.Distance(p.lastPoint, g.CurrentHole.currentPin.FlatPosition) <= Vector2.Distance(preferred.lastPoint, g.CurrentHole.currentPin.FlatPosition))
                {
                    if (p.p < preferred.p || Vector2.Distance(p.lastPoint, g.CurrentHole.currentPin.FlatPosition) < Vector2.Distance(preferred.lastPoint, g.CurrentHole.currentPin.FlatPosition))
                        preferred = p;
                }
            }

            return preferred;
        }
        else
        {
            return new ShotPath();
        }
    }

    public ShotPoint Golfer_CalculateNextShot(Golfer g, Vector2 fromPosition)
    {
        if (CompressedLine.ContainsKey(g.gamemode.getTeebox(g.round.TeeType, g.round.CurrentHole)))
        {
            List<Vector3> listRaw = TargetLine[g.gamemode.getTeebox(g.round.TeeType, g.round.CurrentHole)];

            List<Vector3> list = new List<Vector3>();

            List<ShotPoint> paths = new List<ShotPoint>();

            foreach(Vector3 v in listRaw)
            {
                if (v.Equals(listRaw.Last()))
                    list.Add(g.CurrentHole.currentPin.transform.position);
                else
                    list.Add(new Vector3(v.x + 0.5f, v.y, v.z + 0.5f));
            }

            for (int i = 0; i < list.Count; i++)
            {
                ShotPoint p = new ShotPoint();

                p.cost = getRiskBetweenPoints(g.PlayerBall.FlatPosition, list[i].ToVector2());
                p.point = new Vector2(list[i].x, list[i].z);
                p.distanceToPin = Yard.FloatToYard(Vector2.Distance(p.point, g.CurrentHole.currentPin.FlatPosition));
                p.distanceFromStart = Yard.FloatToYard(Vector2.Distance(p.point, fromPosition));

                if(p.distanceFromStart <= g.Stats.DriverLength)
                    paths.Add(p);
            }

            ShotPoint best = paths[0];

            foreach (ShotPoint path in paths)
            {
                if (path.cost > g.Personality.MaxShotCost)
                    continue;

                if (path.distanceToPin < best.distanceToPin)
                    best = path;
            }

            return best;
        }

        else return new ShotPoint();
    }

    [System.Obsolete]
    private ShotPath getLowestPriceShot(List<ShotPath> paths)
    {
        float lowestPrice = float.PositiveInfinity;
        ShotPath lowest = new ShotPath();

        foreach (ShotPath p in paths)
        {
            if (p.p < lowestPrice)
            {
                lowestPrice = p.p;
                lowest = p;
            }
        }

        return lowest;
    }

    private float getRiskBetweenPoints(Vector2 start, Vector2 target)
    {
        Vector2 dir = (start - target).normalized;
        float r = 0;

        float shotRiskPenalty = family.GetChunkDataPointGroundTypeGlobally((int)start.x, (int)target.y).shotFromRiskPenalty;

        for(int i = 0; i <= (int)Vector2.Distance(start, target); i++)
        {
            Vector2 check = start + (dir * i);

            GA.Ground.GroundType type = family.GetChunkDataPointGroundTypeGlobally((int)check.x, (int)check.y);

            r += type.shotRisk * (shotRiskPenalty + 1);
        }

        return r;
    }

    List<Vector2> listVector3ToVector2(List<Vector3> list)
    {
        List<Vector2> l = new List<Vector2>();

        foreach(Vector3 v in list)
        {
            l.Add(new Vector2(v.x, v.z));
        }

        return l;
    }

    bool IsBitSet(byte b, int pos)
    {
        return (b & (1 << pos)) != 0;
    }

    public void Construction_CalculateTargetLine()
    {
        foreach (Tees t in TeesList)
        {
            GA.Pathfinding.AStarBallFinder finder = new GA.Pathfinding.AStarBallFinder(t.transform.position.ToVector2(), currentPin.transform.position.ToVector2(), family);
            //TargetLine.Add(t, finder.FindPath());
            StartCoroutine(finder.FindPath(FinishedCalculatingTargetLine, t));
        }
    }

    GA.Pathfinding.AStarBallFinder last;
    public void Construction_CalculateTempLine(Tees t, Pin p)
    {
        Debug.DrawRay(t.Position, Vector3.up * 100, Color.black, 10f);

        if (last != null)
            last.Abort = true;

        GA.Pathfinding.AStarBallFinder finder = new GA.Pathfinding.AStarBallFinder(new Vector2(t.Position.x, t.Position.z), new Vector2(p.PositionFine.x, p.PositionFine.z), family);
        //TargetLine.Add(t, finder.FindPath());
        StartCoroutine(finder.FindPath(FinishedCalculatingTargetLine, t));
        last = finder;
    }

    private void CompressLine(Tees t)
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

                    if (Yard.FloatToYard(distance) >= 20) //put a shootable check here
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

    private void FinishedCalculatingTargetLine(List<Vector3> vectors, Tees t)
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
            if (t.Fencing)
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

    }

    public void OnValidation()
    {

    }

    public void OnOpen()
    {
        gamemode.HoleList.Add(this);
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
        lr.material = lineMaterial;
        lr.startColor = Color.white;
        lr.endColor = Color.white;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        return lr;
    }
}

[System.Obsolete]
public struct ShotPath
{
    public float y;
    public List<float> yList;
    public float r;
    public List<Vector2> points;

    public Vector2 lastPoint
    {
        get
        {
            return points[points.Count - 1];
        }
    }

    public int shotCount;
    public float p
    {
        get
        {
            return y + r;
        }
    }
}

public struct ShotPoint
{
    public float cost;
    public Vector2 point;
    public float distanceToPin;
    public float distanceFromStart;
}
