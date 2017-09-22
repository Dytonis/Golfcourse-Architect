using GA.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Pathfinding.Ballfinding
{
    public partial class BallPathFinder : MonoBehaviour //DEFAULT
    {
        public RailMotion motion;
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

            motion.Standard.CalculateRail(transform.position, velocity, 0, 0);
        }

        protected float lastPointDistance(RailPoint[] points, Vector3 target, out Vector3 point)
        {
            float distance = Vector3.Distance(points.Last().point, target);
            point = points.Last().point;
            return distance;
        }

        protected float getAngleFromSpeed(float speed)
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
        public RailPoint[] rail;
        public Vector3 closestPoint;
        public RailPoint lastPoint
        {
            get
            {
                return rail.Last();
            }
        }
        public bool isEndingInHole
        {
            get
            {
                return rail.Last().inHole;
            }
        }
    }
}
