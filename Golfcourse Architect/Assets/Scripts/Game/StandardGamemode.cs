using GA.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GA.Game.AI;

public class StandardGamemode : MonoBehaviour
{
    public int TimeMultiplier = 50;
    public int TimeOfDay;
    public string TimeOfDayString;
    public Transform Sun;

    public Clubhouse clubhouse;
    public ChunkFamily family;

    public Golfer GolferPrefab;

    public List<Golfer> Golfers = new List<Golfer>();
    public List<Hole> HoleList = new List<Hole>();
    public int GolferCap = 10;
    public int CooldownGolferSpawn = 60;
    public float GolferSpawnChance = 0.01f;

    public bool Started = false;
    
    public Vector3[] PositionsForAllCurrentHoles
    {
        get
        {
            List<Vector3> list = new List<Vector3>();

            foreach(Hole h in HoleList)
            {
                list.Add(h.currentPin.transform.position);
            }

            return list.ToArray();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">The type of hole the golfer prefers</param>
    /// <param name="hole">The current hole 1 based</param>
    /// <returns></returns>
    public Tees getTeebox(TeeTypes type, int hole)
    {
        if (HoleList.Count >= hole - 1)
        {
            Dictionary<int, int> types = new Dictionary<int, int>();

            int index = 0;
            foreach(Tees t in HoleList[hole - 1].TeesList)
            {
                types.Add((int)t.TeeType, index);
                index++;
            }

            if ((int)type == 0)
                return HoleList[hole - 1].TeesList[0];

            if (types.ContainsKey((int)type))
                return HoleList[hole - 1].TeesList[types[(int)type]];
            else
            {
                for(int i = 1; i < 8; i++)
                {
                    if (types.ContainsKey((int)type - i))
                        return HoleList[hole - 1].TeesList[types[(int)type - i]];
                }
            }
        }

        return null;
    }

    public void StartGame()
    {
        if(!Started)
            StartCoroutine(cycle());

        Started = true;
        TimeOfDay = 15000;
    }

    private IEnumerator cycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            Tick();
        }
    }


	public void Tick() //10 hz global clock cycle
    {
        AdvanceTime();

        TimeOfDayString = GetTimeOfDayString();

        if(HoleList.Count > 0)
        {
            SpawnGolfersWhenAble();
        }
    }

    private void AdvanceTime()
    {
        TimeOfDay++;

        if(TimeOfDay == 1440 * TimeMultiplier)
        {
            TimeOfDay = 0;
        }

        Sun.transform.eulerAngles = new Vector3(GetSunAngleFromTimeofDay(), 0, 0);
    }

    public string GetTimeOfDayString()
    {
        bool AM = true;

        int hour = (int)(TimeOfDay / (60f * TimeMultiplier));
        int minute = (int)(((TimeOfDay / (60f * TimeMultiplier)) % 1) * 60);

        if (hour > 12)
        {
            hour -= 12;
            AM = false;
        }

        if(AM)
        {
            return hour + ":" + minute.ToString("00") + " AM";
        }
        else
        {
            return hour + ":" + minute.ToString("00") + " PM";
        }
    }

    public float GetSunAngleFromTimeofDay()
    {
        return (360 * ((float)TimeOfDay / (1440f * TimeMultiplier))) - 105;
    }

    private void SpawnGolfersWhenAble()
    {
        if (CooldownGolferSpawn == 0)
        {
            if (Random.Range(0f, 1f) < GolferSpawnChance)
            {
                if (Golfers.Count < GolferCap)
                {
                    CooldownGolferSpawn = 1500;
                    Golfer g = Instantiate(GolferPrefab, clubhouse.GolferInitialSpawnPoint.transform.position, GolferPrefab.transform.rotation) as Golfer;
                    g.Init();
                    g.gamemode = this;
                    g.clubhouse = clubhouse;
                    g.family = family;
                    g.round.TeeTime = TimeOfDay + Random.Range(300, 1000);
                    g.StartAI();
                    g.State = new AIStateArriving();
                    Golfers.Add(g);

                    g.StartToMoveToPoint(new Vector2(clubhouse.InitialMoveSpot.transform.position.x, clubhouse.InitialMoveSpot.transform.position.z));
                    clubhouse.OpenDoorThenClose(2);
                }
            }
        }
        else
        {
            CooldownGolferSpawn--;
        }
    }
}

