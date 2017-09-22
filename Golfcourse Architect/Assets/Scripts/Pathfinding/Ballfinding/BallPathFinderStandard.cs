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
        private const float DistanceChange = 0.01f;
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

            float distance = 0;
            float oppDistance = 0;

            for (int i = 0; i < iterations; i++)
            {
                Vector3 dirToLast = Math.Direction(start, last.closestPoint);

                float angleDelta = Math.AngleDir(dir, dirToLast, Vector3.up);

                Vector3 dirFromBallToTarget = Math.FlatDirection(last.closestPoint, target);
                Vector3 dirFromBallToStart = Math.FlatDirection(last.closestPoint, start);
                float angleToTarget = Vector3.Angle(dirFromBallToTarget, dirFromBallToStart);
                float distanceFromStartToBall = Math.FlatDistance(start, last.closestPoint);

                //last.angle += -angleDelta / 2;

                //Debug.Log(distance);

                if (angleDelta < 0)
                {
                    oppDistance *= -1;
                }
                if (Vector3.Dot(dirFromBallToStart, dirFromBallToTarget) < 0)
                {
                    //ball is closer than target
                    distance *= -1;
                }
                Debug.Log("Opp: " + oppDistance + ", Distance: " + distance);
                last.angle += (3f * -oppDistance);
                last.speed += DistanceChange * -distance;

                last = getPathLine_Standard(last.speed, last.angle, start, target, dir);

                distance = Mathf.Cos(Math.DegreesToRadians(angleToTarget)) * Math.FlatDistance(last.closestPoint, target);
                oppDistance = Mathf.Sin(Math.DegreesToRadians(angleToTarget)) * Math.FlatDistance(last.closestPoint, target);

                Debug.DrawRay(last.closestPoint + (dirFromBallToStart.normalized * distance), dirFromBallToStart.normalized * -distance, Color.yellow, speed);
                Debug.DrawRay(last.closestPoint + (dirFromBallToStart.normalized * distance), (Quaternion.AngleAxis(90, Vector3.up) * (dirFromBallToStart.normalized * -oppDistance)), Color.cyan, speed);
                Debug.DrawRay(last.closestPoint, Vector3.up, Color.white, speed);

                float d = Math.FlatDistance(last.closestPoint, target);

                if (d < closestDistance)
                {
                    closestDistance = d;
                    closest = last;
                }
                else if (last.isEndingInHole)
                {
                    closest = last;
                    break;
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

            float maxVel = maxVelocity;

            float distance = 0;
            float oppDistance = 0;

            for (int i = 0; i < iterations; i++)
            {
                Vector3 dirToLast = Math.Direction(start, last.closestPoint);

                float angleDelta = Math.AngleDir(dir, dirToLast, Vector3.up);

                Vector3 dirFromBallToTarget = Math.FlatDirection(last.closestPoint, target);
                Vector3 dirFromBallToStart = Math.FlatDirection(last.closestPoint, start);
                float angleToTarget = Vector3.Angle(dirFromBallToTarget, dirFromBallToStart);
                float distanceFromStartToBall = Math.FlatDistance(start, last.closestPoint);

                //last.angle += -angleDelta / 2;

                //Debug.Log(distance);

                if (angleDelta < 0)
                {
                    oppDistance *= -1;
                }
                if (gap > distanceFromStartToBall)
                {
                    //ball is closer than target
                    distance *= -1;
                }
                last.angle += (3f * -oppDistance);
                last.speed += (DistanceChange * -distance);

                last = getPathLine_Standard(last.speed, last.angle, start, target, dir);

                distance = Mathf.Cos(Math.DegreesToRadians(angleToTarget)) * Math.FlatDistance(last.closestPoint, target);
                oppDistance = Mathf.Sin(Math.DegreesToRadians(angleToTarget)) * Math.FlatDistance(last.closestPoint, target);

                Debug.DrawRay(last.closestPoint + (dirFromBallToStart.normalized * distance), dirFromBallToStart.normalized * -distance, Color.yellow, speed);
                Debug.DrawRay(last.closestPoint + (dirFromBallToStart.normalized * distance), (Quaternion.AngleAxis(90, Vector3.up) * (dirFromBallToStart.normalized * -oppDistance)), Color.cyan, speed);
                Debug.DrawRay(last.closestPoint, Vector3.up, Color.white, speed);

                float d = Vector3.Distance(last.closestPoint, new Vector3(target.x, target.y - 0.1f, target.z));

                if (d < closestDistance)
                {
                    closestDistance = d;
                    closest = last;
                }
                else if (last.isEndingInHole)
                {
                    closest = last;
                    break;
                }
            }

            motion.drawDebug = true;
            motion.Standard.CalculateRail(start, last.velocity, 0, 0);

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
            block.closestPoint = closestPoint;
            block.rail = rail;

            return block;
        }
        #endregion
    }
}
