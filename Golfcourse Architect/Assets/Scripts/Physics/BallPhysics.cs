using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Physics
{
    public class BallPhysics
    {
        public virtual RailPoint[] CalculateRail(Vector3 startingPosition, Vector3 startingVelocity, float spin, float sideSpin) { throw new NotImplementedException(); }

        protected Vector3 Bounce(Vector3 velIn, Vector3 normal, float r, float f)
        {
            Vector3 u = ((Vector3.Dot(velIn, normal) / normal.sqrMagnitude) * normal);
            Vector3 w = velIn - u;

            Vector3 vel = (f * w) - (r * u);

            return vel;
        }

        protected SlopePackage GetAccelTowards(Vector3 pos, Vector3 direction, float backup = 0.1f, float distance = 200, float gravity = 9.81f)
        {
            Vector3 normalUnder = Vector3.up;
            RaycastHit hit;
            bool h = false;
            Chunk c = null;

            Vector3 newPos = pos + (-direction.normalized * backup);

            if (UnityEngine.Physics.Raycast(newPos, direction.normalized, out hit, distance))
            {
                normalUnder = hit.normal;
                h = true;
                c = hit.collider.GetComponent<Chunk>();
                Debug.DrawRay(hit.point, Vector3.up * 0.3f, Color.magenta, 1f);
            }

            Debug.DrawRay(newPos, direction.normalized * distance, Color.red, 1f);
            Debug.DrawRay(newPos, Vector3.up * 0.15f, Color.red, 1f);
            Debug.DrawRay(pos, Vector3.up * 0.15f, Color.yellow, 1f);

            float angle = Vector3.Angle(normalUnder, Vector3.up);

            float sinOfAngle = Mathf.Sin((angle * Mathf.PI) / 180);

            float accel = sinOfAngle * gravity;
            Vector3 accelDirection = new Vector3(normalUnder.x, 0, normalUnder.z).normalized;

            GA.Ground.GroundType type = new GA.Ground.Rough_Standard();

            if (c != null)
            {
                Vector2 v = c.globalXYToVertex(hit.point.x, hit.point.z);

                type = c.data[(int)v.x, (int)v.y].type;

                Vector2 v2 = c.getGlobalPointFromLocal(v.x, v.y);
            }

            return new SlopePackage()
            {
                dirNormalized = accelDirection,
                normal = hit.normal.normalized,
                hit = hit,
                magnitude = accel,
                detected = h,
                groundType = type,
            };
        }
    }
}
