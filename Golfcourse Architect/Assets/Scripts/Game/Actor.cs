using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public Vector2 FlatPosition
    {
        get
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }

    public Vector2 Velocity;
    public float MaxSpeed;
    public float Acceleration;
    public float Brake;

    public bool Moving
    {
        get
        {
            if (Velocity.magnitude < 0.1f)
                return false;
            else return true;
        }
    }

    public void StartToPathToPoints(Vector2[] points)
    {
        StartCoroutine(PathToPoints(points));
    }

    private IEnumerator PathToPoints(Vector2[] points)
    {
        int index = 0;
        foreach (Vector2 point in points)
        {
            while (Vector2.Distance(point, FlatPosition) > 0.1f)
            {
                Vector2 dir = (FlatPosition - point).normalized;

                Velocity += (-dir * (Acceleration * Time.deltaTime));

                if (Velocity.magnitude > MaxSpeed)
                {
                    Velocity.Normalize();
                    Velocity *= MaxSpeed;
                }

                transform.position = new Vector3(transform.position.x + (Velocity.x * Time.deltaTime), transform.position.y, transform.position.z + (Velocity.y * Time.deltaTime));

                Ray r = new Ray(new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), Vector3.down);

                RaycastHit h;

                int mask = LayerMask.NameToLayer("Terrain");

                if (Physics.Raycast(r, out h, 30, 1 << mask))
                {
                    transform.position = new Vector3(transform.position.x, h.point.y, transform.position.z);
                }

                yield return new WaitForEndOfFrame();
            }

            if(index == points.Length - 2)
            {
                float stoppingDistance = 0.1f;

                while (Vector2.Distance(point, FlatPosition) > stoppingDistance)
                {
                    Vector2 dir = (FlatPosition - point).normalized;

                    Velocity += (-dir * (Acceleration * Time.deltaTime));

                    if (Velocity.magnitude > MaxSpeed)
                    {
                        Velocity.Normalize();
                        Velocity *= MaxSpeed;
                    }

                    transform.position = new Vector3(transform.position.x + (Velocity.x * Time.deltaTime), transform.position.y, transform.position.z + (Velocity.y * Time.deltaTime));

                    Ray r = new Ray(new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), Vector3.down);

                    RaycastHit h;

                    int mask = LayerMask.NameToLayer("Terrain");

                    if (Physics.Raycast(r, out h, 30, 1 << mask))
                    {
                        transform.position = new Vector3(transform.position.x, h.point.y, transform.position.z);
                    }

                    stoppingDistance = (Velocity.magnitude * Velocity.magnitude) / (2 * Brake);

                    yield return new WaitForEndOfFrame();
                }

                while (Velocity.magnitude > 0.1)
                {
                    float mag = Velocity.magnitude;
                    Velocity.Normalize();
                    Velocity *= (mag - (Brake * Time.deltaTime));

                    transform.position = new Vector3(transform.position.x + (Velocity.x * Time.deltaTime), transform.position.y, transform.position.z + (Velocity.y * Time.deltaTime));

                    yield return new WaitForEndOfFrame();
                }

                Velocity = Vector2.zero;
                yield break;
            }
            index++;
        }
    }

    public void StartToMovePoint(Vector2 point)
    {
        StartCoroutine(MoveToPoint(point));
        StartCoroutine(LookTowardsPoint(point));
    }

    public void StartToPathToPoint(Vector2 point, ChunkFamily family)
    {
        GA.Pathfinding.AStarGolferFinder golferFinder = new GA.Pathfinding.AStarGolferFinder(FlatPosition, point, family);
        golferFinder.FindPath(PathFound);
    }

    private void PathFound(Vector2[] points)
    {
        StartToPathToPoints(points);
    }

    private IEnumerator LookTowardsPoint(Vector2 point)
    {
        float elapsedTime = 0;
        float time = 0.3f;

        Quaternion startingRotation = transform.rotation;
        Vector3 D = new Vector3(point.x - FlatPosition.x, 0, point.y - FlatPosition.y);

        Debug.DrawRay(new Vector3(FlatPosition.x, transform.position.y, FlatPosition.y), Vector3.up * 100, Color.red, 10f);
        Debug.DrawRay(new Vector3(point.x, transform.position.y, point.y), Vector3.up * 100, Color.red, 10f);
        Debug.DrawRay(new Vector3(FlatPosition.x, transform.position.y, FlatPosition.y), D, Color.white, 10f);

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime; // <- move elapsedTime increment here
            // Rotations
            transform.rotation = Quaternion.Slerp(startingRotation, Quaternion.LookRotation(D.normalized), (elapsedTime / time));

            transform.eulerAngles = new Vector3(0, transform.eulerAngles.x + transform.eulerAngles.y, 0);

            Debug.Log(Quaternion.LookRotation(D.normalized).eulerAngles);

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator MoveToPoint(Vector2 point)
    {
        float stoppingDistance = 0.1f;

        while (Vector2.Distance(point, FlatPosition) > stoppingDistance)
        {
            Vector2 dir = (FlatPosition - point).normalized;

            Velocity += (-dir * (Acceleration * Time.deltaTime));

            if (Velocity.magnitude > MaxSpeed)
            {
                Velocity.Normalize();
                Velocity *= MaxSpeed;
            }

            transform.position = new Vector3(transform.position.x + (Velocity.x * Time.deltaTime), transform.position.y, transform.position.z + (Velocity.y * Time.deltaTime));

            Ray r = new Ray(new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), Vector3.down);

            RaycastHit h;

            int mask = LayerMask.NameToLayer("Terrain");

            if(Physics.Raycast(r, out h, 30, 1 << mask))
            {
                transform.position = new Vector3(transform.position.x, h.point.y, transform.position.z);
            }

            stoppingDistance = (Velocity.magnitude * Velocity.magnitude) / (2 * Brake);

            yield return new WaitForEndOfFrame();
        }

        while (Velocity.magnitude > 0.1)
        {
            float mag = Velocity.magnitude;
            Velocity.Normalize();
            Velocity *= (mag - (Brake * Time.deltaTime));

            transform.position = new Vector3(transform.position.x + (Velocity.x * Time.deltaTime), transform.position.y, transform.position.z + (Velocity.y * Time.deltaTime));

            yield return new WaitForEndOfFrame();
        }

        Velocity = Vector2.zero;
    }
}
