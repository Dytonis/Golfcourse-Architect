using GA.Pathfinding.Ballfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Physics
{
    public class RailMotion : MonoBehaviour
    {
        public List<RailPoint> Rail = new List<RailPoint>();

        public Vector3 testVelocity;
        public Transform testTarget;

        public bool drawDebug = false;

        public float TimeScale = 1;

        public BallMotionStandard Standard = new BallMotionStandard();
        public BallMotionPutt Putt = new BallMotionPutt();

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

            foreach (RailPoint p in rail)
            {
                if (drawDebug)
                    Debug.Log("Velocity: " + p.velocity + " [" + p.velocity.magnitude + "]");

                RailPoint newPoint = p.Copy();
                if(newPoint.clamped)
                {
                    newPoint.point = clampToGround(newPoint);
                }

                while (Vector3.Dot(newPoint.velocity.normalized, Math.Direction(transform.position, newPoint.point)) > 0)
                {
                    Vector3 direction = Math.Direction(ball.transform.position, newPoint.point);

                    ball.Velocity = direction.normalized * newPoint.velocity.magnitude * (TimeScale * 10);
                    ball.transform.position += ball.Velocity * Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                if (newPoint.frozen)
                {
                    ball.Velocity = Vector3.zero;
                    yield break;
                }
            }
        }

        private Vector3 clampToGround(RailPoint p)
        {
            RaycastHit hit;

            if(UnityEngine.Physics.Raycast(p.point, Vector3.down, out hit, 0.3f, LayerMask.GetMask("Terrain")))
            {
                return hit.point;
            }

            return p.point;
        }
    }

    [System.Serializable]
    public struct RailPoint
    {
        internal Vector3 point;
        internal Vector3 velocity;
        internal bool frozen;
        internal bool grounded;
        internal bool clamped;
        internal bool inHole;

        internal RailPoint Copy()
        {
            return new RailPoint()
            {
                point = this.point,
                velocity = this.velocity,
                grounded = this.grounded,
                clamped = this.clamped,
                frozen = this.frozen,
                inHole = this.inHole
            };
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(RailMotion))]
    public class RailMotionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RailMotion ball = target as RailMotion;

            if (GUILayout.Button("Find Path"))
            {
                ball.GetComponent<BallPathFinder>().StartFindPathIE_Standard(ball.transform.position, ball.testTarget.transform.position);
            }
            if (GUILayout.Button("Find Putt"))
            {
                ball.GetComponent<BallPathFinder>().StartFindPathIE_Putt(ball.transform.position, ball.testTarget.transform.position);
            }
            if (GUILayout.Button("Find Putt then Move"))
            {
                BallPathBlock block = ball.GetComponent<BallPathFinder>().FindPath_Putt(ball.transform.position, ball.testTarget.transform.position);
                ball.StartMoveBallOnRail(block.rail.ToList(), ball.GetComponent<Ball>());
            }
        }
    }
#endif
}
