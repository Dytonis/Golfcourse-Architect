﻿using GA.Game;
using System.Collections;
using System.Collections.Generic;
using GA;
using UnityEngine;
using GA.Physics;

public partial class Golfer : Actor
{
    public Clubhouse clubhouse;
    [SerializeField]
    public Round round;

    public Ball BallPrefab;
    public GameObject TeePrefab;

    public Ball PlayerBall;
    public RailMotion ballMotion;
    public BallPathSolver solver;

    [SerializeField]
    public PlayerPhysique Physique;
    [SerializeField]
    public GolferStats Stats;
    [SerializeField]
    public GolferPersonality Personality;

    public Hole CurrentHole
    {
        get
        {
            if (family != null)
            {
                if (gamemode.HoleList.Count >= round.CurrentHole)
                {
                    return gamemode.HoleList[round.CurrentHole - 1];
                }
            }
            return null;
        }
    }

    public bool IsNextHole
    {
        get
        {
            if(family != null)
            {
                if(gamemode.HoleList.Count >= round.CurrentHole + 1)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public Tees CurrentTees
    {
        get
        {
            return gamemode.getTeebox(round.TeeType, round.CurrentHole);
        }
    }

    public void Init()
    {
        round = new Round();
    }

    public void SpawnBall(Vector3 position, Quaternion rot, bool tee = true)
    {
        PlayerBall = Instantiate<Ball>(BallPrefab, position, rot) as Ball;
        ballMotion = PlayerBall.GetComponent<RailMotion>();
        if(tee)
        {
            Instantiate(TeePrefab, position, rot);
        }
    }

    #region ANIMATION_EVENTS
    private bool _reachDown;
    public void AE_ReachDown()
    {
        Debug.Log("AnimationEvent: AE_ReachDown");

        _reachDown = true;
    }

    public bool AEST_ReachDown()
    {
        if (_reachDown)
        {
            _reachDown = false;
            return true;
        }
        else return false;
    }

    private bool _swingBottom;
    public void AE_SwingBottom()
    {
        Debug.Log("AnimationEvent: AE_SwingBottom");

        _swingBottom = true;
    }

    public bool AEST_SwingBottom()
    {
        if (_swingBottom)
        {
            _swingBottom = false;
            return true;
        }
        else return false;
    }
    #endregion
}

[System.Serializable]
public struct GolferStats
{
    [SerializeField]
    public PlayerSkillLevel SkillLevelTag;
    [Range(0, 40)]
    public int Handicap;
    [Range(0, 350)]
    public float DriverLength;
    [Range(0, 100)]
    public float LongAccuracy;
    [Range(0, 100)]
    public float ShortAccuracy;
    [Range(0, 100)]
    public float PuttAccuracy;
    [Range(0, 100)]
    public float FadeAccuracy;
    [Range(0, 100)]
    public float DrawAccuracy;
    [Range(0, 100)]
    public float RoughAccuracy;
    [Range(0, 100)]
    public float BunkerAccuracy;
    [Range(0, 100)]
    public float PitchAccuracy;
    [Range(0, 100)]
    public float ChipAccuracy;
    [Range(0, 100)]
    public float FlopAccuracy;
    [Range(0, 100)]
    public float SpinAccuracy;
    [Range(0, 100)]
    public float SliceChance;
    [Range(0, 100)]
    public float HookChance;
    [Range(0, 100)]
    public float ShankChance;
    [Range(0, 100)]
    public float FatChance;
    [Range(0, 100)]
    public float ThinChance;
}

[System.Serializable]
public struct GolferPersonality
{
    [Range(0, 100)]
    public float Aggresivness;
    [Range(0, 100)]
    public float Creativeness;
    [Range(0, 100)]
    public float Temperment;
    [Range(0, 100)]
    public float Patience;
    [Range(-100, 100)]
    public float Mood;
    [Range(0, 100)]
    public float Hunger;
    [Range(0, 100)]
    public float Thirst;
    [Range(0, 100)]
    public float Bathroom;
    public float MaxShotCost
    {
        get
        {
            float a = Math.NormalizeRange(Aggresivness, 0, 100, 0, 850);
            float c = Math.NormalizeRange(Creativeness, 0, 100, 0, 350);

            float part1 = a + c;

            float t = 1 - Math.Normalize(Temperment, 0, 100); //inverse temperment normalized. higher temperment = less mood affects the msc
            float m = Mathf.Abs(Math.NormalizeRange(Mood, -100, 100, -1, 1) * 1000) * t;

            float part2 = m + part1;
            return Mathf.Round(Math.NormalizeRange(part2, 0, 2000, 250, 1000));
        }
    }
}

[System.Serializable]
public enum Gender
{
    Male,
    Female
}

[System.Serializable]
public enum Age
{
    Child,
    YoungAdult,
    Adult,
    Senior
}

[System.Serializable]
public struct PlayerPhysique
{
    public Gender Gender;
    public Age Age;
    public byte AgeByte;
}

[System.Serializable]
public enum PlayerSkillLevel
{
    Hobbyist,
    Novice,
    Amature,
    Scratch,
    Pro,
    TouringPro,
    WorldChampion,
}