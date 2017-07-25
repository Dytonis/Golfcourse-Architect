using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ClubStat
{
    public virtual string uiname { get; set;  }

    public virtual float launchAngle { get; set; }
    public virtual float backspin { get; set; }
    /// <summary>
    /// The distance that under it, all players will never choose this club.
    /// </summary>
    public virtual float distanceMin { get { return 0; } }
    /// <summary>
    /// The distance that above it, all players will never choose this club.
    /// </summary>
    public virtual float distanceMax { get { return float.PositiveInfinity; } }
    /// <summary>
    /// The distance that a max level golfer will hit it.
    /// </summary>
    public virtual float distanceAvg10 { get; set; }
    /// <summary>
    /// The distance that a level 1 golfer will hit it.
    /// </summary>
    public virtual float distanceAvg1 { get; set; }

    public virtual void OnBallContact() { }

    public static float GetSpin(int level, float distance)
    {
        float maxSpin = GetClubFromDistance(level, distance).backspin;
        float normalizedLevel = (float)((float)level - 1f) / (float)(998f);

        float spin = (maxSpin * 0.5f) + ((maxSpin * 0.5f) * normalizedLevel);

        return spin;
    }

    public static float GetSpin(ClubStat C, int level)
    {
        float maxSpin = C.backspin;
        float normalizedLevel = (float)((float)level - 1f) / (float)(998f);

        float spin = (maxSpin * 0.5f) + ((maxSpin * 0.5f) * normalizedLevel);

        return spin;
    }

    public static ClubStat GetClubFromDistance(int level, float distance)
    {
        float normalizedLevel = (float)((float)level - 1f) / (float)(998f);

        foreach(ClubStat c in ListOfAllClubs.Reverse())
        {
            float distanceMax = c.distanceAvg1 + ((c.distanceAvg10 - c.distanceAvg1) * normalizedLevel);

            if(distance <= distanceMax)
            {
                return c;
            }
        }

        return new Wood_1();
    }

    public static ClubStat[] ListOfAllClubs
    {
        get
        {
            return new List<ClubStat>()
            {
                new Wood_1(),
                new Wood_2(),
                new Wood_3(),
                new Wood_4(),
                new Wood_5(),
                new Wood_6(),
                new Wood_7(),
                new Hybrid_1(),
                new Hybrid_2(),
                new Hybrid_3(),
                new Hybrid_4(),
                new Hybrid_5(),
                new Hybrid_6(),
                new Iron_1(),
                new Iron_2(),
                new Iron_3(),
                new Iron_4(),
                new Iron_5(),
                new Iron_6(),
                new Iron_7(),
                new Iron_8(),
                new Iron_9(),
                new Wedge_Pitching(),
                new Wedge_Gap(),
                new Wedge_Sand(),
                new Wedge_Lob(),
                new Wedge_UltraLob(),
            }.ToArray();
        }
    }
}

public class Wood_1 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic Driver";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 230;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 300;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 2600;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 11f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wood_2 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 2 Wood";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 220;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 280;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 3200;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 10f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wood_3 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 3 Wood";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 210;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 270;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 3600;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 9f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wood_4 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic Driver";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4100;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 200;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 265;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 9.2f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wood_5 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 5 wood";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 195;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 260;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4600;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 9.5f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wood_6 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 6 wood";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 190;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 255;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4700;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 10.5f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wood_7 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 7 wood";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 180;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 250;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4800;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 11f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Hybrid_1 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 1 Iron Hybrid";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 170;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 230;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4400;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 10f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Hybrid_2 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 2 Iron Hybrid";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 165;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 225;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4450;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 10.2f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Hybrid_3 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 3 Iron Hybrid";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 160;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 220;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4550;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 10.5f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Hybrid_4 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 4 Iron Hybrid";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 150;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 210;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4600;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 11f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Hybrid_5 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 5 Iron Hybrid";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 145;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 200;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 5000;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 12f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Hybrid_6 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 6 Iron Hybrid";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 137.5f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 190;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 5750;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 14f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Iron_1 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 1 Iron";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 170f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 225;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 3900;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 9.5f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Iron_2 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 2 Iron";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 165f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 220;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4200;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 10f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Iron_3 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 3 Iron";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 160f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 215;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4600;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 10f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Iron_4 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 4 Iron";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 150f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 205;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 4800;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 20f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Iron_5 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 5 Iron";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 140f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 195;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 5300;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 22f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Iron_6 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 6 Iron";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 130f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 185;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 6200;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 24f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Iron_7 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 7 Iron";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 120f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 175;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 7000;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 26.3f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Iron_8 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 8 Iron";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 110f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 165;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 8000;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 28.1f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Iron_9 : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic 9 Iron";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 100f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 153;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 8700;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 30.4f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wedge_Pitching : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic Pitching Wedge";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 85f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 140;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 9300;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 34.4f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wedge_Gap : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic Gap Wedge";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 70f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 120;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 9600;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 35f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wedge_Sand : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic Sand Wedge";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 55f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 100;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 11500;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 45f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wedge_Lob : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic Lob Wedge";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 40f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 80;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 10000;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 50f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}
public class Wedge_UltraLob : ClubStat
{
    public override string uiname
    {
        get
        {
            return "Generic Ultra Lob Wedge";
        }

        set
        {
            base.uiname = value;
        }
    }

    public override float distanceAvg1
    {
        get
        {
            return 30f;
        }

        set
        {
            base.distanceAvg1 = value;
        }
    }

    public override float distanceAvg10
    {
        get
        {
            return 60;
        }

        set
        {
            base.distanceAvg10 = value;
        }
    }

    public override float backspin
    {
        get
        {
            return 9000;
        }

        set
        {
            base.backspin = value;
        }
    }

    public override float launchAngle
    {
        get
        {
            return 60f;
        }

        set
        {
            base.launchAngle = value;
        }
    }
}