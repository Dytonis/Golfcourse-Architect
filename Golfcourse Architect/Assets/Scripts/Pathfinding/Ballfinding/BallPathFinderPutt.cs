using GA.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Pathfinding.Ballfinding
{
    public partial class BallPathFinder //PUTT
    {
        #region FinderAlgorithm
        public BallPathBlock FindPathFlatTarget_Putt(Vector3 start, Vector2 target)
        {
            Vector3 target2 = Vector3.zero;

            RaycastHit hit;
            if (UnityEngine.Physics.Raycast(new Vector3(target.x, 100, target.y), Vector3.down, out hit, 200))
            {
                target2 = new Vector3(target.x, hit.point.y, target.y);
            }

            return FindPath_Putt(start, target2);
        }
        public void StartFindPathIE_Putt(Vector3 start, Vector3 target)
        {
            StartCoroutine(FindPathIE_Putt(start, target));
        }
        /*private IEnumerator FindPathIE_Putt(Vector3 start, Vector3 target)
        {
            float e = start.y;
            RaycastHit hit;
            if (UnityEngine.Physics.Raycast(new Vector3(target.x, 100, target.z), Vector3.down, out hit, 200))
            {
                target = new Vector3(target.x, hit.point.y, target.z);
            }

            RaycastHit hit2;
            if (UnityEngine.Physics.Raycast(new Vector3(start.x, 100, start.z), Vector3.down, out hit2, 200))
            {
                start = new Vector3(start.x, hit2.point.y, start.z);
            }

            float gap = Vector3.Distance(start, new Vector3(target.x, start.y, target.z));
            float elevationGain = e - start.y;
            Vector3 heading = new Vector3(target.x, start.y, target.z) - start;
            Vector3 dir = heading / heading.magnitude;

            Debug.DrawRay(new Vector3(target.x, target.y, target.z), Vector3.up, Color.cyan, 100f);
            Debug.DrawRay(start, dir, Color.white, 100f);

            List<BallPathBlock> list = new List<BallPathBlock>();

            Vector3 velocity = Vector3.one;

            for (float d = -15, o = 0; d < 15; d += 0.5f)
            {
                for (float s = 0; s < 1; s += 0.02f, o++)
                {
                    if (o % 25 == 0)
                        yield return new WaitForEndOfFrame();
                    Vector3 starting = start;
                    heading = new Vector3(target.x, start.y, target.z) - starting;
                    dir = heading / heading.magnitude;

                    dir = Quaternion.Euler(new Vector3(0, d, 0)) * dir;

                    Debug.DrawRay(starting, dir * 10, Color.magenta, 10f);

                    velocity = (1 * s) * dir;
                    Vector3 position = starting;
                    list.Clear();

                    BallPathBlock block = getPathLine_Putt(s, d, start, target, Math.Direction(start, dir));

                    list.Add(block);

                    if(Math.FlatDistance(block.lastPoint, target) < 0.1f)
                    {
                        
                        yield break;
                    }
                }
            }
            yield break;
        }*/
        private IEnumerator FindPathIE_Putt(Vector3 start, Vector3 target)
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
                Vector3 dirToLast = Math.Direction(start, last.lastPoint);

                float angleDelta = Math.AngleDir(dir, dirToLast, Vector3.up);

                Vector3 dirFromBallToTarget = Math.FlatDirection(last.lastPoint, target);
                Vector3 dirFromBallToStart = Math.FlatDirection(last.lastPoint, start);
                float angleToTarget = Vector3.Angle(dirFromBallToTarget, dirFromBallToStart);
                float distanceFromStartToBall = Math.FlatDistance(start, last.lastPoint);

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
                Debug.Log("Opp: " + oppDistance + ", Distance: " + distance);
                last.angle += (3f * -oppDistance);
                last.speed += (0.05f * -distance);

                last = getPathLine_Putt(last.speed, last.angle, start, target, dir);

                distance = Mathf.Cos(Math.DegreesToRadians(angleToTarget)) * Math.FlatDistance(last.lastPoint, target);
                oppDistance = Mathf.Sin(Math.DegreesToRadians(angleToTarget)) * Math.FlatDistance(last.lastPoint, target);

                Debug.DrawRay(last.lastPoint + (dirFromBallToStart.normalized * distance), dirFromBallToStart.normalized * -distance, Color.yellow, speed);
                Debug.DrawRay(last.lastPoint + (dirFromBallToStart.normalized * distance), (Quaternion.AngleAxis(90, Vector3.up) * (dirFromBallToStart.normalized * -oppDistance)), Color.cyan, speed);
                Debug.DrawRay(last.lastPoint, Vector3.up, Color.white, speed);

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

        public BallPathBlock FindPath_Putt(Vector3 start, Vector3 target)
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
                Vector3 dirToLast = Math.Direction(start, last.lastPoint);

                float angleDelta = Math.AngleDir(dir, dirToLast, Vector3.up);

                Vector3 dirFromBallToTarget = Math.FlatDirection(last.lastPoint, target);
                Vector3 dirFromBallToStart = Math.FlatDirection(last.lastPoint, start);
                float angleToTarget = Vector3.Angle(dirFromBallToTarget, dirFromBallToStart);
                float distanceFromStartToBall = Math.FlatDistance(start, last.lastPoint);

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
                Debug.Log("Opp: " + oppDistance + ", Distance: " + distance);
                last.angle += (3f * -oppDistance);
                last.speed += (0.05f * -distance);

                last = getPathLine_Putt(last.speed, last.angle, start, target, dir);

                distance = Mathf.Cos(Math.DegreesToRadians(angleToTarget)) * Math.FlatDistance(last.lastPoint, target);
                oppDistance = Mathf.Sin(Math.DegreesToRadians(angleToTarget)) * Math.FlatDistance(last.lastPoint, target);

                Debug.DrawRay(last.lastPoint + (dirFromBallToStart.normalized * distance), dirFromBallToStart.normalized * -distance, Color.yellow, speed);
                Debug.DrawRay(last.lastPoint + (dirFromBallToStart.normalized * distance), (Quaternion.AngleAxis(90, Vector3.up) * (dirFromBallToStart.normalized * -oppDistance)), Color.cyan, speed);
                Debug.DrawRay(last.lastPoint, Vector3.up, Color.white, speed);

                float d = Vector3.Distance(last.lastPoint, new Vector3(target.x, target.y - 0.1f, target.z));

                if (d < closestDistance)
                {
                    closestDistance = d;
                    closest = last;
                }
            }

            motion.drawDebug = true;
            motion.Standard.CalculateRail(start, last.velocity, 0, 0);

            return closest;
        }
        #endregion

        #region railAlgorithm
        private BallPathBlock getPathLine_Putt(float speed, float angle, Vector3 start, Vector3 target, Vector3 dir)
        {
            BallPathBlock block = new BallPathBlock()
            {
                speed = speed,
                angle = angle
            };

            Debug.DrawRay(target, Vector3.up * 100, Color.yellow, 10f);

            Vector3 velocity = speed * dir;
            Vector3 axis = Quaternion.Euler(new Vector3(0, -90, 0)) * dir;
            velocity = Quaternion.AngleAxis(angle, Vector3.up) * velocity;
            block.velocity = velocity;

            Vector3 closestPoint;

            RailPoint[] rail = motion.Putt.CalculateRail(start, velocity, 0, 0);

            float distance = lastPointDistance(rail.ToArray(), target, out closestPoint);
            block.lastPoint = closestPoint;
            block.rail = rail;

            return block;
        }
        #endregion
    }
}