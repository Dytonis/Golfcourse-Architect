using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BallPathSolver : MonoBehaviour
{

    public ChunkFamily family;
    public Vector3 target;

    public Ball ball;
    public Vector3 currentPos;
    public Vector3 startingPos;

    public float a;

    public List<Vector3> points = new List<Vector3>();

    public void Start()
    {
        StartCoroutine(waiter());
        //a += 0.01f * Time.deltaTime;
    }

    public IEnumerator waiter()
    {
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(AttemptToFindPutt(8));
    }

    public void Solved(bool final)
    {
        if(closest > 0.1f && !final)
        {
            StartCoroutine(AttemptToFindPutt(25, final: true, speedRangeMax: 3.5f, speedRes: 0.01f));
        }

        foreach (Vector3 v in closeList)
        {
            if(final)
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
                if(o % 50 == 0)
                    yield return new WaitForSeconds(0);
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

    public IEnumerator AttemptToFindStandard(float checkSize, bool final = false)
    {
        float e = startingPos.y;
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(target.x, 100, target.z), Vector3.down, out hit, 200))
        {
            target = new Vector3(target.x, hit.point.y, target.z);
        }

        float gap = Vector3.Distance(startingPos, new Vector3(target.x, startingPos.y, target.z));
        float elevationGain = e - startingPos.y;
        Vector3 heading = new Vector3(target.x, startingPos.y, target.z) - startingPos;
        Vector3 dir = heading / heading.magnitude;

        Vector3 landPoint = startingPos + (dir * 0.75f * gap);

        Debug.DrawRay(new Vector3(target.x, target.y, target.z), Vector3.up, Color.cyan, 100f);
        Debug.DrawRay(startingPos, dir, Color.white, 100f);
        Debug.DrawRay(landPoint, Vector3.up, Color.yellow, 100f);

        Vector3 velocity = Vector3.one;

        for (float y = -5f; y < 5f; y += 0.2f)
        {
            for (float x = -5f; x < 5f; x += 0.2f)
            {
                yield return new WaitForEndOfFrame();

                Vector3 starting = new Vector3(landPoint.x + x, landPoint.y, landPoint.z + y);
                heading = starting - startingPos;
                dir = heading / heading.magnitude;
                velocity = 1 * dir;
                Vector3 position = starting;

                list.Clear();

                for (int i = 0; i < 30; i++)
                {
                    SlopePackage slope = GetAccelUnder(position.x, position.z);
                    RaycastHit rhit = slope.hit;
                    velocity += slope.dirNormalized * (slope.magnitude / 100);

                    velocity -= (velocity * 0.1f);
                    if (velocity.magnitude <= 0.05f)
                        velocity = Vector3.zero;

                    position += velocity * 1f;

                    position = new Vector3(position.x, rhit.point.y, position.z);

                    list.Add(position);
                    float distance = Vector3.Distance(position, target);
                    if (velocity.magnitude <= 0.05f || i == 29 || distance <= 0.1f)
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


                    if (closest <= 0.1f)
                    {
                        this.points = closeList;
                        Solved(final);
                        yield break;
                    }
                }
            }
        }

        this.points = closeList;
        Solved(final);

        yield break;
    }

    private SlopePackage GetAccelUnder(float x, float y)
    {
        Vector3 normalUnder = Vector3.up;
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(x, 100, y), Vector3.down, out hit, 200))
        {
            normalUnder = hit.normal;
        }
        float angle = Vector3.Angle(normalUnder, Vector3.up);

        float sinOfAngle = Mathf.Sin((angle * Mathf.PI) / 180);

        float accel = sinOfAngle * ball.gravity;
        Vector3 accelDirection = new Vector3(normalUnder.x, 0, normalUnder.z).normalized;
        return new SlopePackage()
        {
            dirNormalized = accelDirection,
            hit = hit,
            magnitude = accel
        };
    }

    private RaycastHit GetRaycastPoint(float x, float y)
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
    public Vector3 dirNormalized;
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
