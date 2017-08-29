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

        public int iterations;

        public float speed = 3;

        public void StartFindPath(Vector3 start, Vector3 target)
        {
            StartCoroutine(FindPath(start, target));
        }

        public void TestVel(Vector3 target)
        {
            Vector3 heading = new Vector3(target.x, transform.position.y, target.z) - transform.position;
            Vector3 dir = heading / heading.magnitude;

            Vector3 velocity = vel * dir;
            Vector3 axis = Quaternion.Euler(new Vector3(0, -90, 0)) * dir;
            velocity = Quaternion.AngleAxis(angle, axis) * velocity;

            physics.CalculateRailFromStats(transform.position, velocity, 0, 0);
        }

        private IEnumerator FindPath(Vector3 start, Vector3 target)
        {
            float gap = -Vector3.Distance(start, new Vector3(target.x, start.y, target.z));
            float elevationGain = target.y - start.y;

            Vector3 heading = new Vector3(target.x, start.y, target.z) - start;
            Vector3 dir = heading / heading.magnitude;

            BallPathFinderDataBlock initial = getBlock(start, target, dir, 0.25f, maxVelocity, -30, 30);

            float closest = float.PositiveInfinity;

            BallPathFinderDataBlock last = initial;
            yield return new WaitForSeconds(speed);

            float leftAngle = -30f;
            float rightAngle = 30f;
            float minVel = 0.25f;
            float maxVel = maxVelocity;

            for (int i = 0; i < iterations; i++)
            {
                //ANGLE
                if (last.TOP_LEFT < last.TOP_RIGHT)
                {
                    //path is more left
                    leftAngle = last.ANGLE_LEFT; //keep left where it is
                    rightAngle = rightAngle - ((Math.AbsDifference(leftAngle, rightAngle) / 2)); //move right angle left by half the size

                    Debug.Log(last.TOP_LEFT * transform.localScale.magnitude);
                    Debug.DrawRay(new Vector3(target.x, 0, target.z), Vector3.up * 100, Color.red, speed);

                    if (last.TOP_LEFT * transform.localScale.magnitude < 0.1f)
                    {
                        //path found
                        yield break;
                    }
                }
                else
                {
                    //path is more right
                    rightAngle = last.ANGLE_RIGHT; //keep right where it is
                    leftAngle = leftAngle + ((Math.AbsDifference(leftAngle, rightAngle) / 2)); //move left angle right by half the size

                    Debug.Log(last.TOP_RIGHT * transform.localScale.magnitude);
                    Debug.DrawRay(new Vector3(target.x, 0, target.z), Vector3.up * 100, Color.red, speed);

                    if (last.TOP_RIGHT * transform.localScale.magnitude < 0.1f)
                    {
                        //path found
                        yield break;
                    }
                }

                //POWER
                if(last.TOP_LEFT < last.BOTTOM_LEFT)
                {
                    //path is more top
                    maxVel = last.VEL_MAX; //keep max where it is
                    minVel = minVel + ((last.VEL_MAX - last.VEL_MIN) / 2); //increase minVel by half the size

                    if (last.TOP_LEFT * transform.localScale.magnitude < 0.1f)
                    {
                        //path found
                        yield break;
                    }
                }
                else
                {
                    //path is more bottom
                    minVel = last.VEL_MIN; //keep min where it is
                    maxVel = maxVel - ((last.VEL_MAX - last.VEL_MIN) / 2); //decrease maxVel by half the size

                    if (last.BOTTOM_LEFT * transform.localScale.magnitude < 0.1f)
                    {
                        //path found
                        yield break;
                    }
                }

                last = getBlock(start, target, dir, minVel, maxVel, leftAngle, rightAngle);
                yield return new WaitForSeconds(speed);
            }
        }

        private BallPathFinderDataBlock getBlock(Vector3 start, Vector3 target, Vector3 dir, float minVel, float maxVel, float leftAngle, float rightAngle)
        {
            BallPathFinderDataBlock block = new BallPathFinderDataBlock();
            block.ANGLE_LEFT = leftAngle;
            block.ANGLE_RIGHT = rightAngle;
            block.VEL_MAX = maxVel;
            block.VEL_MIN = minVel;

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

                    float d = closestPoint(physics.CalculateRailFromStats(start, velocity, 0, 0).ToArray(), target);

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

            return block;
        }
        private float closestPoint(RailPoint[] points, Vector3 target)
        {
            float distance = float.PositiveInfinity;

            foreach(RailPoint p in points)
            {
                if (p.velocity.magnitude > 0.35f)
                    continue;

                float d = Vector3.Distance(p.point, target);

                if (d < distance)
                    distance = d;
            }

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
    }
}
