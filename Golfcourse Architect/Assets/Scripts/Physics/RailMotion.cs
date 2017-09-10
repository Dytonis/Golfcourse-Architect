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

                while (Vector3.Distance(ball.transform.position, p.point) > 0.1f)
                {
                    Vector3 direction = Math.Direction(ball.transform.position, p.point);

                    ball.Velocity = direction.normalized * p.velocity.magnitude * (TimeScale * 10);
                    ball.transform.position += ball.Velocity * Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                if (p.frozen)
                {
                    ball.Velocity = Vector3.zero;
                    yield break;
                }
            }
        }
    }

    [System.Serializable]
    public struct RailPoint
    {
        public Vector3 point;
        public Vector3 velocity;
        public bool frozen;
        public bool grounded;
        public bool bouncing;

        public RailPoint Copy()
        {
            return new RailPoint()
            {
                point = this.point,
                velocity = this.velocity,
                grounded = this.grounded,
                bouncing = this.bouncing,
                frozen = this.frozen
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
        }
    }
#endif
}
