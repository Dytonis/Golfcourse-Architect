using GA.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Pathfinding.Ballfinding
{
    public partial class BallPathFinder //STANDARD
    {
        #region FinderAlgorithm
        public BallPathBlock FindPathFlatTarget_Standard(Vector3 start, Vector2 target)
        {
            Vector3 target2 = Vector3.zero;

            RaycastHit hit;
            if (UnityEngine.Physics.Raycast(new Vector3(target.x, 100, target.y), Vector3.down, out hit, 200))
            {
                target2 = new Vector3(target.x, hit.point.y, target.y);
            }

            return FindPath_Standard(start, target2);
        }
        public void StartFindPathIE_Standard(Vector3 start, Vector3 target)
        {
            StartCoroutine(FindPathIE_Standard(start, target));
        }
        private IEnumerator FindPathIE_Standard(Vector3 start, Vector3 target)
        {
            float gap = -Vector3.Distance(start, new Vector3(target.x, start.y, target.z));
            float elevationGain = target.y - start.y;
            float closestDistance = float.PositiveInfinity;
            Vector3 heading = new Vector3(target.x, start.y, target.z) - start;
            Vector3 dir = heading / heading.magnitude;

            BallPathBlock initial = getPathLine_Standard(maxVelocity, 0, start, target, dir);
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

                last = getPathLine_Standard(last.speed, last.angle, start, target, dir);

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

            motion.drawDebug = true;
            motion.Standard.CalculateRail(start, last.velocity, 0, 0);
            yield break;
        }
        private BallPathBlock FindPath_Standard(Vector3 start, Vector3 target)
        {
            float gap = -Vector3.Distance(start, new Vector3(target.x, start.y, target.z));
            float elevationGain = target.y - start.y;
            float closestDistance = float.PositiveInfinity;
            Vector3 heading = new Vector3(target.x, start.y, target.z) - start;
            Vector3 dir = heading / heading.magnitude;

            BallPathBlock initial = getPathLine_Standard(maxVelocity, 0, start, target, dir);

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

                last = getPathLine_Standard(last.speed, last.angle, start, target, dir);

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
        #endregion

        #region railAlgorithm
        private BallPathBlock getPathLine_Standard(float speed, float angle, Vector3 start, Vector3 target, Vector3 dir)
        {
            BallPathBlock block = new BallPathBlock()
            {
                speed = speed,
                angle = angle
            };

            Debug.DrawRay(target, Vector3.up * 100, Color.yellow, 10f);

            float launchAngle = getAngleFromSpeed(speed);
            Vector3 velocity = speed * dir;
            Vector3 axis = Quaternion.Euler(new Vector3(0, -90, 0)) * dir;
            velocity = Quaternion.AngleAxis(launchAngle, axis) * velocity;
            velocity = Quaternion.AngleAxis(angle, Vector3.up) * velocity;
            block.velocity = velocity;

            Vector3 closestPoint;

            RailPoint[] rail = motion.Standard.CalculateRail(start, velocity, 0, 0);

            float distance = lastPointDistance(rail.ToArray(), target, out closestPoint);
            block.lastPoint = closestPoint;
            block.rail = rail;

            return block;
        }
        #endregion
    }
}
