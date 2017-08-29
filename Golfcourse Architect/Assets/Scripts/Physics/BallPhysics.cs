using GA.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Physics
{
    public class BallPhysics : MonoBehaviour
    {
        public List<RailPoint> Rail = new List<RailPoint>();

        public Vector3 testVelocity;
        public Transform testTarget;

        public float speed = 3;

        public List<RailPoint> CalculateRailFromStats(Vector3 startingPosition, Vector3 startingVelocity, float spin, float sideSpin)
        {
            //raycast from normal velocity by ball size / 2
            //if hit, add point on hit posision and update velocity

            List<RailPoint> rail = new List<RailPoint>();

            RailPoint rp = new RailPoint()
            {
                point = startingPosition,
                velocity = startingVelocity
            };

            for (int i = 0; i < 100; i++)
            {
                rp.velocity = new Vector3(rp.velocity.x, rp.velocity.y - 0.0981f, rp.velocity.z); //gravity
                Vector3 horzVelocity = new Vector3(rp.velocity.x, 0, rp.velocity.z); //get horizontal velocity
                rp.velocity = new Vector3(rp.velocity.x, rp.velocity.y + (horzVelocity.magnitude / 10), rp.velocity.z); //lift
                rp.velocity *= 1 - (0.035f * rp.velocity.magnitude); //drag

                SlopePackage package = GetAccelTowards(rp.point, rp.velocity.normalized, 0.1f, rp.velocity.magnitude);

                if(package.detected)
                {
                    //detected ground

                    if (rp.velocity.y >= -0.3f)
                    {
                        //dont bounce, roll
                        rp.velocity = new Vector3(rp.velocity.x, 0, rp.velocity.z); //remove vertical speed
                        rp.velocity += package.dirNormalized * (package.magnitude / 1000); //accel gained from slope
                        rp.velocity *= package.groundType.friction; //friction
                    }
                    else
                    {
                        rp.point = package.hit.point; //set the position to where it hit the ground
                        rp.velocity = bounce(rp.velocity, package.normal, package.groundType.restitution, package.groundType.friction); //calculate bounce                  
                    }

                    rail.Add(rp.Copy()); //add a point where it touched the ground
                }

                rp.point += rp.velocity;
                rail.Add(rp.Copy());

                Debug.DrawRay(rp.point, rp.velocity, Color.white, speed);

                if (rp.velocity.magnitude < 0.1f)
                    break;
            }

            return rail;
        }

        private Vector3 bounce(Vector3 velIn, Vector3 normal, float r, float f)
        {
            Vector3 u = ((Vector3.Dot(velIn, normal) / normal.sqrMagnitude) * normal);
            Vector3 w = velIn - u;

            Vector3 vel = (f * w) - (r * u);

            return vel;
        }

        private SlopePackage GetAccelTowards(Vector3 pos, Vector3 direction, float backup = 0.1f, float distance = 200, float gravity = 9.81f)
        {
            Vector3 normalUnder = Vector3.up;
            RaycastHit hit;
            bool h = false;
            Chunk c = null;

            Vector3 newPos = pos + (-direction.normalized * backup);

            Debug.DrawRay(newPos, direction.normalized * distance, Color.blue, speed);

            if (UnityEngine.Physics.Raycast(newPos, direction.normalized, out hit, distance))
            {
                normalUnder = hit.normal;
                h = true;
                c = hit.collider.GetComponent<Chunk>();
            }
            float angle = Vector3.Angle(normalUnder, Vector3.up);

            float sinOfAngle = Mathf.Sin((angle * Mathf.PI) / 180);

            float accel = sinOfAngle * gravity;
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
                normal = hit.normal.normalized,
                hit = hit,
                magnitude = accel,
                detected = h,
                groundType = type,
            };
        }
    }

    [System.Serializable]
    public struct RailPoint
    {
        public Vector3 point;
        public Vector3 velocity;

        public RailPoint Copy()
        {
            return new RailPoint()
            {
                point = this.point,
                velocity = this.velocity
            };
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(BallPhysics))]
    public class BallPhysicsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BallPhysics ball = target as BallPhysics;

            if(GUILayout.Button("Test Rail"))
            {
                ball.GetComponent<BallPathFinder>().TestVel(ball.testTarget.transform.position);
            }

            if (GUILayout.Button("Find Path"))
            {
                ball.GetComponent<BallPathFinder>().StartFindPath(ball.transform.position, ball.testTarget.transform.position);
            }
        }
    }
#endif
}
