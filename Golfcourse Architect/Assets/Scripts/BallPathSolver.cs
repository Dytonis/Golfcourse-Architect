using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BallPathSolver : MonoBehaviour
{
    public ChunkFamily family;
    public Vector3 target;
    public int Level = 900;

    public ShotType type;
    public Ball ball;
    public Vector3 currentPos;
    public Vector3 startingPos;

    public float Carry;
    public float TotalDistance;

    public ClubStat ClubSelected;

    [SerializeField]
    public string ClubSelectedName;

    public float a;

    public List<Vector3> points = new List<Vector3>();

    public void Solved(bool final)
    {
        if (closest > 0.1f && !final)
        {
            switch (type)
            {
                case ShotType.Putt:
                    StartCoroutine(AttemptToFindPutt(25, final: true, speedRangeMax: 3.5f, speedRes: 0.01f));
                    break;
                case ShotType.Standard:
                    StartCoroutine(AttemptToFindStandard(15, final: true, speedChecks: 30f, speedRangeMax: 2.9f, solverCount: 100));
                    break;
            }
        }

        foreach (Vector3 v in closeList)
        {
            if (final)
                Debug.DrawRay(v, Vector3.up, Color.black, 100f);
            else
                Debug.DrawRay(v, Vector3.up, Color.gray, 100f);
        }
    }

    public float closest = float.PositiveInfinity;
    List<Vector3> list = new List<Vector3>();
    List<Vector3> closeList = new List<Vector3>();

    public IEnumerator AttemptToFindPutt(float checkSize = 8, float speedRes = 0.05f, float angleRes = 0.5f, float speedRangeMax = 2f, bool final = false)
    {
        float e = startingPos.y;
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(target.x, 100, target.z), Vector3.down, out hit, 200))
        {
            target = new Vector3(target.x, hit.point.y, target.z);
        }

        RaycastHit hit2;
        if (Physics.Raycast(new Vector3(startingPos.x, 100, startingPos.z), Vector3.down, out hit2, 200))
        {
            startingPos = new Vector3(startingPos.x, hit2.point.y, startingPos.z);
        }

        float gap = Vector3.Distance(startingPos, new Vector3(target.x, startingPos.y, target.z));
        float elevationGain = e - startingPos.y;
        Vector3 heading = new Vector3(target.x, startingPos.y, target.z) - startingPos;
        Vector3 dir = heading / heading.magnitude;

        Debug.DrawRay(new Vector3(target.x, target.y, target.z), Vector3.up, Color.cyan, 100f);
        Debug.DrawRay(startingPos, dir, Color.white, 100f);


        Vector3 velocity = Vector3.one;

        for (float d = -checkSize, o = 0; d < checkSize; d += angleRes)
        {
            for (float s = 0; s < speedRangeMax; s += speedRes, o++)
            {
                if (o % 40 == 0)
                    yield return new WaitForEndOfFrame();
                Vector3 starting = startingPos;
                heading = new Vector3(target.x, startingPos.y, target.z) - starting;
                dir = heading / heading.magnitude;

                dir = Quaternion.Euler(new Vector3(0, d, 0)) * dir;

                Debug.DrawRay(starting, dir * 10, Color.magenta, 10f);

                velocity = (1 * s) * dir;
                Vector3 position = starting;
                list.Clear();

                for (int i = 0; i < 30; i++)
                {
                    SlopePackage slope = GetAccelUnder(position.x, position.z);
                    RaycastHit rhit = slope.hit;
                    velocity += slope.dirNormalized * (slope.magnitude / 100);

                    velocity -= (velocity * 0.1f);
                    if (velocity.magnitude <= 0.01f)
                        velocity = Vector3.zero;

                    position += velocity * 1f;

                    position = new Vector3(position.x, rhit.point.y, position.z);

                    if (position.x < 0)
                        break;
                    if (position.z < 0)
                        break;

                    if (position.x > family.globalSize.x)
                        break;
                    if (position.z > family.globalSize.y)
                        break;

                    list.Add(position);
                    float distance = Vector3.Distance(position, target);
                    if ((velocity.magnitude <= 0.15f && distance <= 0.1f) || i == 29)
                    {
                        if (distance < closest)
                        {
                            closeList.Clear();

                            foreach (Vector3 v in list)
                                closeList.Add(v);

                            closest = distance;
                            break;
                        }
                    }
                }

                if (position.x < 0)
                    break;
                if (position.z < 0)
                    break;

                if (position.x > family.globalSize.x)
                    break;
                if (position.z > family.globalSize.y)
                    break;

                if (closest <= 0.1f)
                {
                    this.points = closeList;
                    Solved(final);
                    yield break;
                }
            }

        }

        this.points = closeList;
        Solved(final);

        yield break;
    }

    public void StartAttempToFindStandard()
    {
        float gap = Vector3.Distance(startingPos, new Vector3(target.x, startingPos.y, target.z));

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(target.x, 100, target.z), Vector3.down, out hit, 200))
        {
            target = new Vector3(target.x, hit.point.y, target.z);
        }

        RaycastHit hit2;
        if (Physics.Raycast(new Vector3(startingPos.x, 100, startingPos.z), Vector3.down, out hit2, 200))
        {
            startingPos = new Vector3(startingPos.x, hit2.point.y, startingPos.z);
        }

        Vector3 heading = new Vector3(target.x, startingPos.y, target.z) - startingPos;
        Vector3 dir = (heading / heading.magnitude).normalized;
        float elevationGain = startingPos.y - target.y;
        Vector3 carryLocation = target - (dir * (gap * 0.05f));
        float CarryDistance = Vector3.Distance(startingPos, carryLocation);
        Carry = Yard.FloatToYard(CarryDistance);
        TotalDistance = Yard.FloatToYard(gap);

        ClubSelected = ClubStat.GetClubFromDistance(Level, Carry);
        ClubSelectedName = ClubSelected.uiname;

        Debug.DrawRay(carryLocation, Vector3.up, Color.yellow, 100f);

        StartCoroutine(AttemptToFindStandard(10, solverCount: 100, speedRangeMax: 2.9f, speedChecks: 20f));
    }

    private IEnumerator AttemptToFindStandard(float angularSize = 8, float speedChecks = 0.05f, int angleChecks = 16, float speedRangeMax = 2f, int solverCount = 60, bool final = false)
    {
        float e = startingPos.y;
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(target.x, 100, target.z), Vector3.down, out hit, 200))
        {
            target = new Vector3(target.x, hit.point.y, target.z);
        }

        RaycastHit hit2;
        if (Physics.Raycast(new Vector3(startingPos.x, 100, startingPos.z), Vector3.down, out hit2, 200))
        {
            startingPos = new Vector3(startingPos.x, hit2.point.y, startingPos.z);
        }

        float gap = Vector3.Distance(startingPos, new Vector3(target.x, startingPos.y, target.z));
        float elevationGain = e - startingPos.y;
        Vector3 heading = new Vector3(target.x, startingPos.y, target.z) - startingPos;
        Vector3 dir = heading / heading.magnitude;

        Debug.DrawRay(new Vector3(target.x, target.y, target.z), Vector3.up, Color.cyan, 100f);
        Debug.DrawRay(startingPos, dir, Color.white, 100f);

        Vector3 velocity = Vector3.one;
        velocity = Quaternion.AngleAxis(ClubSelected.launchAngle, Vector3.right) * velocity;
        float spin = 0;

        float checkSize = Mathf.Atan(angularSize / (gap * 2)) * (180f / Mathf.PI); //half the angular size in degrees given final size angularSize
        float angleRes = (checkSize * 2) / angleChecks; //the entire angular size included in resolution calculation

        float sStart;
        float sMax;

        FindSpeedRange(out sStart, out sMax);

        float res = (sMax - sStart) / speedChecks;

        for (float d = -checkSize, o = 0; d < checkSize; d += angleRes)
        {
            for (float s = sStart, sn = (s - sStart) / (sMax - sStart); s < sMax; s += res, o++, sn = (s - sStart) / (sMax - sStart))
            {
                if (o % 20 == 0)
                  yield return new WaitForSeconds(0);
                if (sn > 1)
                    sn = 1;

                Vector3 starting = startingPos;
                heading = new Vector3(target.x, startingPos.y, target.z) - starting;
                dir = heading / heading.magnitude;

                dir = Quaternion.Euler(new Vector3(0, d, 0)) * dir;

                Debug.DrawRay(starting, dir * 10, Color.magenta, 10f);

                velocity = ((100 / gap) * s) * dir;
                velocity = new Vector3(velocity.x, 0, velocity.z);
                Vector3 axis = Quaternion.Euler(new Vector3(0, -90, 0)) * dir;
                velocity = Quaternion.AngleAxis(ClubSelected.launchAngle, axis) * velocity;
                spin = ClubStat.GetSpin(ClubSelected, Level);
                spin = spin * sn;
                Vector3 position = starting;
                list.Clear();
                bool detectedLastFrame = false;

                for (int i = 0; i < solverCount; i++)
                {
                    SlopePackage slope = GetAccelTowards(position, velocity, 0f, 0.25f + velocity.magnitude);
                    RaycastHit rhit = slope.hit;

                    Debug.DrawRay(position, velocity, Color.white, 0.001f);

                    position += velocity;

                    if (slope.detected)
                    {
                        //GROUNDED
                        velocity += slope.dirNormalized * (slope.magnitude / 100);
                        position.Set(position.x, slope.hit.point.y, position.z);
                        velocity *= 1 - slope.groundType.friction;

                        //bounce
                        if (velocity.magnitude > 0.5f)
                        {
                            Debug.DrawRay(slope.hit.point, slope.hit.normal, Color.red, 0.001f);

                            Vector3 invNomral = Quaternion.AngleAxis(180, -slope.hit.normal) * -velocity;

                            Debug.DrawRay(slope.hit.point, invNomral, Color.green, 0.001f);

                            velocity = invNomral * slope.groundType.restitution;
                            velocity.Set(velocity.x, velocity.y * (slope.groundType.restitution), velocity.z);

                            Debug.DrawRay(slope.hit.point, velocity, Color.blue, 0.001f);
                        }
                        else if (velocity.magnitude > 0.2f)
                            velocity.Set(velocity.x, 0, velocity.z);
                        else
                            velocity = Vector3.zero;
                        /////////

                        if (detectedLastFrame == false)
                        {
                            //first frame detection
                            //position += velocity * 1f;
                            position.Set(slope.hit.point.x, slope.hit.point.y, slope.hit.point.z);
                            list.Add(position);
                            Debug.DrawRay(position, Vector3.up * ((spin + 500) / 5000f), Color.blue, 0.001f);
                        }
                        else
                        {
                            //continued
                            velocity -= dir * (spin / 80000f);
                            spin -= spin * 0.075f;
                            if (spin < 1000)
                                spin = 0;
                        }
                    }
                    else
                    {
                        //IN THE AIR
                        velocity.Set(velocity.x, velocity.y -= (9.81f / 100f), velocity.z);
                        spin -= spin * (0.02f);
                        float spinAffector = ((spin) - 4000) / 4000;
                        if (spinAffector < 0)
                            spinAffector = 0;

                        velocity.Set(velocity.x, velocity.y + (((spinAffector * 0.5f) + 0.5f) * (new Vector3(velocity.x, 0, velocity.z).sqrMagnitude / 20)), velocity.z);
                        velocity.Set(velocity.x, velocity.y -= (9.81f / 100f), velocity.z);


                        velocity -= (velocity.normalized * (0.02f) * (velocity.sqrMagnitude)); //air resistance
                    }
                    detectedLastFrame = slope.detected;

                    if (velocity.magnitude <= 0.01f)
                        velocity = Vector3.zero;

                    if (position.x < 0)
                        break;
                    if (position.z < 0)
                        break;

                    if (position.x > family.globalSize.x)
                        break;
                    if (position.z > family.globalSize.y)
                        break;

                    list.Add(position);
                    float distance = Vector3.Distance(position, target);
                    if (((velocity.magnitude <= 0.15f && distance <= 0.1f) || i == solverCount - 1) || velocity.magnitude <= 0.01f)
                    {
                        if (distance < closest)
                        {
                            closeList.Clear();

                            foreach (Vector3 v in list)
                                closeList.Add(v);

                            closest = distance;
                            break;
                        }
                    }
                }

                if (closest <= 0.1f)
                {
                    this.points = closeList;
                    Solved(final);
                    yield break;
                }
            }
        }
        this.points = closeList;
        Solved(final);
        yield break;
    }

    private void FindSpeedRange(out float sStart, out float sMax)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(target.x, 100, target.z), Vector3.down, out hit, 200))
        {
            target = new Vector3(target.x, hit.point.y, target.z);
        }

        RaycastHit hit2;
        if (Physics.Raycast(new Vector3(startingPos.x, 100, startingPos.z), Vector3.down, out hit2, 200))
        {
            startingPos = new Vector3(startingPos.x, hit2.point.y, startingPos.z);
        }

        float gap = Vector3.Distance(startingPos, new Vector3(target.x, startingPos.y, target.z));
        Vector3 heading = new Vector3(target.x, startingPos.y, target.z) - startingPos;
        Vector3 dir = heading / heading.magnitude;
        float elevationGain = startingPos.y - target.y;
        Vector3 carryLocation = heading / (1.05f + (elevationGain / 25f));
        float CarryDistance = Vector3.Distance(startingPos, carryLocation);
        Debug.DrawRay(new Vector3(target.x, target.y, target.z), Vector3.up, Color.cyan, 100f);
        Debug.DrawRay(startingPos, dir, Color.white, 100f);

        Vector3 velocity = Vector3.one;
        velocity = Quaternion.AngleAxis(ClubSelected.launchAngle, Vector3.right) * velocity;
        float spin = 0;

        float start = 0;
        float max = 5;

        float recordedStart = float.PositiveInfinity;
        float recordedMax = float.PositiveInfinity;

        float res = 0.2f;

        for (float s = start, sn = (s - start) / (max - start); s < max; s += res, sn = (s - start) / (max - start))
        {
            //if (o % 50 == 0)
            //  yield return new WaitForEndOfFrame();
            if (sn > 1)
                sn = 1;

            Vector3 starting = startingPos;
            heading = new Vector3(target.x, startingPos.y, target.z) - starting;
            dir = heading / heading.magnitude;

            Debug.DrawRay(starting, dir * 10, Color.magenta, 10f);

            velocity = ((100 / gap) * s) * dir;
            velocity = new Vector3(velocity.x, 0, velocity.z);
            Debug.DrawRay(startingPos, velocity.normalized, Color.red, 10f);
            Vector3 axis = Quaternion.Euler(new Vector3(0, -90, 0)) * dir;
            velocity = Quaternion.AngleAxis(ClubSelected.launchAngle, axis) * velocity;
            Debug.DrawRay(startingPos, velocity.normalized, Color.red, 10f);
            spin = ClubStat.GetSpin(ClubSelected, Level);
            spin = spin * sn;
            Vector3 position = starting;
            list.Clear();
            bool detectedLastFrame = false;

            for (int i = 0; i < 250; i++)
            {
                SlopePackage slope = GetAccelTowards(position, velocity, 0f, 0.25f + velocity.magnitude);
                RaycastHit rhit = slope.hit;

                Debug.DrawRay(position, velocity, Color.white, 0.001f);

                position += velocity;

                if (slope.detected)
                {
                    float distanceStart = Vector3.Distance(position, startingPos);

                    if (distanceStart > CarryDistance / 2 && recordedStart == float.PositiveInfinity)
                        recordedStart = s;
                    if (distanceStart > CarryDistance * 1.25f && recordedMax == float.PositiveInfinity)
                        recordedMax = s;

                    break;
                }
                else
                {
                    //IN THE AIR
                    velocity.Set(velocity.x, velocity.y -= (9.81f / 100f), velocity.z);
                    spin -= spin * (0.02f);
                    float spinAffector = ((spin) - 4000) / 4000;
                    if (spinAffector < 0)
                        spinAffector = 0;

                    velocity.Set(velocity.x, velocity.y + (((spinAffector * 0.5f) + 0.5f) * (new Vector3(velocity.x, 0, velocity.z).sqrMagnitude / 20)), velocity.z);
                    velocity.Set(velocity.x, velocity.y -= (9.81f / 100f), velocity.z);


                    velocity -= (velocity.normalized * (0.02f) * (velocity.sqrMagnitude)); //air resistance
                }
                detectedLastFrame = slope.detected;

                if (velocity.magnitude <= 0.01f)
                    velocity = Vector3.zero;

                Debug.DrawRay(position, Vector3.up * ((spin + 500) / 5000f), Color.yellow, 1f);
            }
        }

        sStart = recordedStart;
        sMax = recordedMax;
    }

    private SlopePackage GetAccelUnder(float x, float y, float elevation = 100, float distance = 200)
    {
        Vector3 normalUnder = Vector3.up;
        RaycastHit hit;
        bool h = false;
        Chunk c = null;
        if (Physics.Raycast(new Vector3(x, elevation, y), Vector3.down, out hit, distance))
        {
            normalUnder = hit.normal;
            h = true;
            c = hit.collider.GetComponent<Chunk>();
        }
        float angle = Vector3.Angle(normalUnder, Vector3.up);

        float sinOfAngle = Mathf.Sin((angle * Mathf.PI) / 180);

        float accel = sinOfAngle * ball.gravity;
        Vector3 accelDirection = new Vector3(normalUnder.x, 0, normalUnder.z).normalized;

        GA.Ground.GroundType type = new GA.Ground.Rough_Standard();

        if (c != null)
        {
            Vector2 v = c.globalXYToVertex(hit.point.x, hit.point.z);
            type = c.data[(int)v.x, (int)v.y].type;
        }

        return new SlopePackage()
        {
            dirNormalized = accelDirection,
            hit = hit,
            magnitude = accel,
            detected = h,
            groundType = type,
        };
    }
    private SlopePackage GetAccelTowards(Vector3 pos, Vector3 direction, float backup = 0.1f, float distance = 200)
    {
        Vector3 normalUnder = Vector3.up;
        RaycastHit hit;
        bool h = false;
        Chunk c = null;

        Vector3 newPos = pos + (-direction.normalized * backup);

        if (Physics.Raycast(newPos, direction.normalized, out hit, distance))
        {
            normalUnder = hit.normal;
            h = true;
            c = hit.collider.GetComponent<Chunk>();
        }
        float angle = Vector3.Angle(normalUnder, Vector3.up);

        float sinOfAngle = Mathf.Sin((angle * Mathf.PI) / 180);

        float accel = sinOfAngle * ball.gravity;
        Vector3 accelDirection = new Vector3(normalUnder.x, 0, normalUnder.z).normalized;

        GA.Ground.GroundType type = new GA.Ground.Rough_Standard();

        if (c != null)
        {
            Vector2 v = c.globalXYToVertex(hit.point.x, hit.point.z);
            type = c.data[(int)v.x, (int)v.y].type;
        }

        return new SlopePackage()
        {
            dirNormalized = accelDirection,
            hit = hit,
            magnitude = accel,
            detected = h,
            groundType = type,
        };
    }

    private RaycastHit GetRaycastPoint(float x, float y, float distance = 200)
    {
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(x, 15, y), Vector3.down, out hit, 200))
        {
            return hit;
        }

        return hit;
    }

    void RecursiveFindLandingTarget(Vector3[] points, Vector3 velocity, Vector3 direction, int iteration = 0, float startingElevation = 0)
    {
        Vector3 pos = new Vector3(target.x, startingElevation, target.z);

        Vector3 normalUnder = Vector3.up;
        RaycastHit hit;

        Ray r = new Ray(new Vector3(target.x, 6, target.z), Vector3.down);
        Debug.DrawRay(new Vector3(target.x, 6, target.z), Vector3.down * 200, Color.yellow, 100f);
        if (Physics.Raycast(r, out hit, 200))
        {
            normalUnder = hit.normal;
        }

        List<Vector3> list = points.ToList();

        foreach (Vector3 v in list)
        {
            if (Physics.Raycast(new Vector3(v.x, 100, v.z), Vector3.down, out hit, 200))
            {
                normalUnder = hit.normal;
            }

            pos = new Vector3(v.x, hit.point.y, v.z);

            Debug.DrawRay(pos, Vector3.up, Color.red, 0.01f);
        }

        float angle = Vector3.Angle(normalUnder, Vector3.up);

        float sinOfAngle = Mathf.Sin((angle * Mathf.PI) / 180);

        float accel = sinOfAngle * ball.gravity;

        Vector3 accelDirection = new Vector3(normalUnder.x, 0, normalUnder.z).normalized;

        velocity += (accel * accelDirection) + (direction * (float)(a * iteration));

        //velocity += 0.6f * accelDirection;

        Vector3 newPoint = pos - (velocity * 0.01f);

        list.Add(newPoint);

        this.points = list;

        if (velocity.magnitude < 300 && iteration < 50)
            RecursiveFindLandingTarget(list.ToArray(), velocity, direction, ++iteration);
    }
}

public struct SlopePackage
{
    public GA.Ground.GroundType groundType;
    public bool detected;
    public Vector3 dirNormalized;
    public Vector3 normal;
    public float magnitude;
    public RaycastHit hit;
}

public enum ShotType
{
    Standard,
    Flop,
    Chip,
    OutAndBack,
    Putt
}
