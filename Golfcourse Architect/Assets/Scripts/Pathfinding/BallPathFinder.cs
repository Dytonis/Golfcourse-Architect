using GA.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Pathfinding
{
    public class BallPathFinder : MonoBehaviour
    {
        public BallPhysics physics;
        public float maxVelocity = 3;
        public float maxAngle = 30;

        public float angle;
        public float vel;

        public float accuracy;

        public int iterations;

        public float speed = 3;

        public void TestVel(Vector3 target)
        {
            Vector3 heading = new Vector3(target.x, transform.position.y, target.z) - transform.position;
            Vector3 dir = heading / heading.magnitude;

            Vector3 velocity = vel * dir;
            Vector3 axis = Quaternion.Euler(new Vector3(0, -90, 0)) * dir;
            velocity = Quaternion.AngleAxis(angle, axis) * velocity;

            physics.CalculateRailFromStats(transform.position, velocity, 0, 0);
        }

        public BallPathBlock FindPathFlatTarget(Vector3 start, Vector2 target)
        {
            Vector3 target2 = Vector3.zero;

            RaycastHit hit;
            if (UnityEngine.Physics.Raycast(new Vector3(target.x, 100, target.y), Vector3.down, out hit, 200))
            {
                target2 = new Vector3(target.x, hit.point.y, target.y);
            }

            return FindPath(start, target2);
        }

        public void StartFindPathIE(Vector3 start, Vector3 target)
        {
            StartCoroutine(FindPathIE(start, target));
        }

        private IEnumerator FindPathIE(Vector3 start, Vector3 target)
        {
            float gap = -Vector3.Distance(start, new Vector3(target.x, start.y, target.z));
            float elevationGain = target.y - start.y;
            float closestDistance = float.PositiveInfinity;
            Vector3 heading = new Vector3(target.x, start.y, target.z) - start;
            Vector3 dir = heading / heading.magnitude;

            BallPathBlock initial = getPathLine(maxVelocity, 0, start, target, dir);
            yield return new WaitForSeconds(speed);

            BallPathBlock last = initial;

            BallPathBlock closest;

            float maxVel = maxVelocity;

            for (int i = 0; i < iterations; i++)
            {
                Vector3 dirToLast = Math.Direction(start, last.lastPoint);

                float angleDelta = Math.AngleDir(dir, dirToLast, Vector3.up);

                Vector3 dirFromBallToTarget = Math.FlatDirection(last.lastPoint, target);
                Vector3 dirFromBallToStart = Math.FlatDirection(last.lastPoint, start);
                float angleToTarget = Vector3.Angle(dirFromBallToTarget, dirFromBallToStart);
                float distanceFromStartToBall = Math.FlatDistance(start, last.lastPoint);

                //last.angle += -angleDelta / 2;

                float distance = Mathf.Cos(Math.DegreesToRadians(angleToTarget)) * Math.FlatDistance(last.lastPoint, target);
                float oppDistance = Mathf.Sin(Math.DegreesToRadians(angleToTarget)) * distance;

                if(angleDelta > 0)
                {
                    //right
                    last.angle -= oppDistance;
                }
                else
                {
                    last.angle += oppDistance;
                }

                if (gap > distanceFromStartToBall)
                {
                    //ball is closer than target
                    distance *= -1;
                }

                Debug.Log("Opp: " + oppDistance + ", Distance: " + distance);
                //Debug.Log(distance);

                last.speed += (0.01f * -distance);

                last = getPathLine(last.speed, last.angle, start, target, dir);

                Debug.DrawRay(last.lastPoint, dirFromBallToStart.normalized * distance, Color.yellow, speed);
                Debug.DrawRay(last.lastPoint, Quaternion.AngleAxis(90, Vector3.up) * dirFromBallToTarget.normalized * oppDistance, Color.cyan, speed);

                Debug.DrawLine(last.lastPoint, Vector3.up, Color.black, speed);

                float d = Math.FlatDistance(last.lastPoint, target);

                if (d < closestDistance)
                {
                    closestDistance = d;
                    closest = last;
                }
                yield return new WaitForSeconds(speed);
            }

            physics.drawDebug = true;
            physics.CalculateRailFromStats(start, last.velocity, 0, 0);
            yield break;
        }

        private BallPathBlock FindPath(Vector3 start, Vector3 target)
        {
            float gap = -Vector3.Distance(start, new Vector3(target.x, start.y, target.z));
            float elevationGain = target.y - start.y;
            float closestDistance = float.PositiveInfinity;
            Vector3 heading = new Vector3(target.x, start.y, target.z) - start;
            Vector3 dir = heading / heading.magnitude;

            BallPathBlock initial = getPathLine(maxVelocity, 0, start, target, dir);

            BallPathBlock last = initial;

            BallPathBlock closest = new BallPathBlock();

            for (int i = 0; i < iterations; i++)
            {
                Vector3 dirToLast = Math.Direction(start, last.lastPoint);

                float angleDelta = Math.AngleDir(dir, dirToLast, Vector3.up);

                Vector3 dirFromBallToTarget = Math.FlatDirection(last.lastPoint, target);
                Vector3 dirFromBallToStart = Math.FlatDirection(last.lastPoint, start);
                float angleToTarget = Vector3.Angle(dirFromBallToTarget, dirFromBallToStart);
                float distanceFromStartToBall = Math.FlatDistance(start, last.lastPoint);

                //last.angle += -angleDelta / 2;

                float distance = Mathf.Cos(Math.DegreesToRadians(angleToTarget)) * Math.FlatDistance(last.lastPoint, target);
                float oppDistance = Mathf.Sin(Math.DegreesToRadians(angleToTarget)) * distance;

                if (angleDelta > 0)
                {
                    //right
                    last.angle -= oppDistance;
                }
                else
                {
                    last.angle += oppDistance;
                }

                if (gap > distanceFromStartToBall)
                {
                    //ball is closer than target
                    distance *= -1;
                }

                Debug.Log("Opp: " + oppDistance + ", Distance: " + distance);
                //Debug.Log(distance);

                last.speed += (0.01f * -distance);

                last = getPathLine(last.speed, last.angle, start, target, dir);

                Debug.DrawRay(last.lastPoint, dirFromBallToStart.normalized * distance, Color.yellow, speed);
                Debug.DrawRay(last.lastPoint, Quaternion.AngleAxis(90, Vector3.up) * dirFromBallToTarget.normalized * oppDistance, Color.cyan, speed);

                Debug.DrawLine(last.lastPoint, target, Color.black, speed);

                float d = Math.FlatDistance(last.lastPoint, target);

                if (d < closestDistance)
                {
                    closestDistance = d;
                    closest = last;
                }
            }

            return closest;
        }

        [Obsolete]
        private BallPathFinderDataBlock getBlock(Vector3 start, Vector3 target, Vector3 dir, float minVel, float maxVel, float leftAngle, float rightAngle)
        {
            BallPathFinderDataBlock block = new BallPathFinderDataBlock();
            block.ANGLE_LEFT = leftAngle;
            block.ANGLE_RIGHT = rightAngle;
            block.VEL_MAX = maxVel;
            block.VEL_MIN = minVel;

            List<Vector3> list = new List<Vector3>();

            //find region
            int i = 0;
            for (float p = minVel, o = 0; o < 2; p = maxVel, o++)
            {
                float launchAngle = getAngleFromSpeed(p);

                for (float a = leftAngle, y = 0; y < 2; a = rightAngle, i++, y++)
                {
                    Vector3 velocity = p * dir;
                    Vector3 axis = Quaternion.Euler(new Vector3(0, -90, 0)) * dir;
                    velocity = Quaternion.AngleAxis(launchAngle, axis) * velocity;
                    velocity = Quaternion.AngleAxis(a, Vector3.up) * velocity;
                    block.velocity = velocity;

                    Vector3 close;

                    float d = lastPointDistance(physics.CalculateRailFromStats(start, velocity, 0, 0).ToArray(), target, out close);

                    list.Add(close);

                    switch (i)
                    {
                        case 0:
                            block.BOTTOM_LEFT = d;
                            break;
                        case 1:
                            block.BOTTOM_RIGHT = d;
                            break;
                        case 2:
                            block.TOP_LEFT = d;
                            break;
                        case 3:
                            block.TOP_RIGHT = d;
                            break;
                    }
                }
            }

            Debug.DrawLine(list[0], list[1], Color.black, speed);
            Debug.DrawLine(list[1], list[3], Color.black, speed);
            Debug.DrawLine(list[2], list[3], Color.black, speed);
            Debug.DrawLine(list[2], list[0], Color.black, speed);

            return block;
        }

        BallPathBlock getPathLine(float speed, float angle, Vector3 start, Vector3 target, Vector3 dir)
        {
            BallPathBlock block = new BallPathBlock()
            {
                speed = speed,
                angle = angle
            };

            float launchAngle = getAngleFromSpeed(speed);
            Vector3 velocity = speed * dir;
            Vector3 axis = Quaternion.Euler(new Vector3(0, -90, 0)) * dir;
            velocity = Quaternion.AngleAxis(launchAngle, axis) * velocity;
            velocity = Quaternion.AngleAxis(angle, Vector3.up) * velocity;
            block.velocity = velocity;

            Vector3 closestPoint;

            List<RailPoint> rail = physics.CalculateRailFromStats(start, velocity, 0, 0);

            float distance = lastPointDistance(rail.ToArray(), target, out closestPoint);
            block.lastPoint = closestPoint;
            block.rail = rail;

            return block;
        }
        private float closestPoint(RailPoint[] points, Vector3 target)
        {
            float distance = float.PositiveInfinity;

            foreach(RailPoint p in points)
            {
                if (p.velocity.magnitude > 0.15f)
                    continue;

                if (p.bouncing)
                    continue;

                float d = Vector3.Distance(p.point, target);

                if (d < distance)
                    distance = d;
            }

            return distance;
        }
        public float closestPoint(RailPoint[] points, Vector3 target, out Vector3 point)
        {
            float distance = float.PositiveInfinity;
            Vector3 closest = Vector3.zero;

            foreach (RailPoint p in points)
            {
                if (p.velocity.magnitude > 0.15f)
                    continue;

                if (p.bouncing)
                    continue;

                float d = Vector3.Distance(p.point, target);

                if (d < distance)
                {
                    distance = d;
                    closest = p.point;
                }
            }
            point = closest;
            return distance;
        }

        private float lastPointDistance(RailPoint[] points, Vector3 target, out Vector3 point)
        {
            float distance = Vector3.Distance(points.Last().point, target);
            point = points.Last().point;
            return distance;
        }

        private float getAngleFromSpeed(float speed)
        {
            float angle = Math.InverseNormalizeRange(speed, 0, maxVelocity, 2, 55);
            return angle;
        }
    }

    public struct BallPathFinderDataBlock
    {
        public float TOP_LEFT;
        public float TOP_RIGHT;
        public float BOTTOM_LEFT;
        public float BOTTOM_RIGHT;
        public float ANGLE_LEFT;
        public float ANGLE_RIGHT;
        public float VEL_MIN;
        public float VEL_MAX;
        public Vector3 velocity;
    }

    public struct BallPathBlock
    {
        public float speed;
        public Vector3 velocity;
        public float angle;
        public List<RailPoint> rail;
        public Vector3 lastPoint;
    }
}
