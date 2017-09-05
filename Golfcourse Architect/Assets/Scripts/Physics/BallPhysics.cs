using GA.Pathfinding;
using System;
using System.Collections;
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

        public bool drawDebug = false;

        public float speed = 3;

        public float TimeScale = 1;

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

            for (int i = 0; i < 250; i++)
            {
                Vector3 oldVelocity = rp.velocity;

                rp.velocity = new Vector3(rp.velocity.x, rp.velocity.y - 0.0381f, rp.velocity.z); //gravity
                Vector3 horzVelocity = new Vector3(rp.velocity.x, 0, rp.velocity.z); //get horizontal velocity
                rp.velocity = new Vector3(rp.velocity.x, rp.velocity.y + (horzVelocity.magnitude / 10), rp.velocity.z); //lift
                rp.velocity *= 1 - (0.05f * rp.velocity.magnitude); //drag

                SlopePackage package = GetAccelTowards(rp.point, rp.velocity.normalized, 0.25f, rp.velocity.magnitude + 0.25f);

                if(package.detected)
                {
                    //detected ground

                    if (rp.velocity.y >= -0.1f)
                    {
                        //dont bounce, roll
                        rp.velocity = new Vector3(rp.velocity.x, 0, rp.velocity.z); //remove vertical speed
                        rp.velocity += package.dirNormalized * (package.magnitude / 100); //accel gained from slope
                        rp.velocity *= package.groundType.friction; //friction
                        rp.bouncing = false;
                    }
                    else
                    {
                        rp.velocity = bounce(rp.velocity, package.normal, package.groundType.restitution, package.groundType.friction); //calculate bounce                  
                        rp.bouncing = true;
                    }

                    rp.point = package.hit.point; //set the position to where it hit the ground
                    rp.grounded = true;

                    RailPoint groundPoint = rp.Copy();
                    groundPoint.velocity = oldVelocity;

                    rail.Add(groundPoint); //add a point where it touched the ground
                }
                else
                {
                    rp.grounded = false;
                }

                if (drawDebug)
                    Debug.DrawRay(rp.point, rp.velocity, Color.white, speed);

                RailPoint point = rp.Copy();
                point.velocity = oldVelocity;

                rail.Add(point); //add a point

                rp.point += rp.velocity;
                if (rp.velocity.magnitude < 0.03f && rp.grounded)
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

        public void StartMoveBallOnRail(List<RailPoint> rail, Ball ball)
        {
            StartCoroutine(EnumerationMoveBallOnRail(rail, ball));
        }

        private IEnumerator EnumerationMoveBallOnRail(List<RailPoint> rail, Ball ball)
        {
            foreach(RailPoint p in rail)
            {
                Debug.DrawRay(p.point, p.velocity, Color.white, 3f);
            }

            ball.transform.position = rail[0].point;

            foreach(RailPoint p in rail)
            {
                if (drawDebug)
                    Debug.Log("Velocity: " + p.velocity + " [" + p.velocity.magnitude + "]");

                while (Vector3.Distance(ball.transform.position, p.point) > 0.1f)
                {
                    Vector3 direction = Math.Direction(ball.transform.position, p.point);

                    ball.Velocity = direction.normalized * p.velocity.magnitude * (TimeScale * 10);
                    ball.transform.position += ball.Velocity * Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }

            if (ball.Velocity.magnitude < 0.35f)
                ball.Velocity = Vector3.zero;
        }

        private SlopePackage GetAccelTowards(Vector3 pos, Vector3 direction, float backup = 0.1f, float distance = 200, float gravity = 9.81f)
        {
            Vector3 normalUnder = Vector3.up;
            RaycastHit hit;
            bool h = false;
            Chunk c = null;

            Vector3 newPos = pos + (-direction.normalized * backup);

            if(drawDebug)
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

                Debug.DrawRay(hit.point, Vector3.up, Color.green, speed);

                type = c.data[(int)v.x, (int)v.y].type;

                Vector2 v2 = c.getGlobalPointFromLocal(v.x, v.y);

                if (drawDebug)
                    Debug.DrawRay(new Vector3(v2.x, 0, v2.y), Vector3.up * 10, Color.magenta, speed);
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
        public bool grounded;
        public bool bouncing;

        public RailPoint Copy()
        {
            return new RailPoint()
            {
                point = this.point,
                velocity = this.velocity,
                grounded = this.grounded,
                bouncing = this.bouncing
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
                ball.GetComponent<BallPathFinder>().StartFindPathIE(ball.transform.position, ball.testTarget.transform.position);
            }
        }
    }
#endif
}
