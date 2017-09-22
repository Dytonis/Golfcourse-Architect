using GA.Physics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace GA.Game.AI
{
    [System.Obsolete]
    public class AIStateHitTeeShot : AIState
    {
        List<RailPoint> points;

        public AIStateHitTeeShot(List<RailPoint> rail)
        {
            points = rail;
        }

        public override IEnumerator EnumerationOnBecameActiveState()
        {
            //chance to move backwards and look (maybe scale that based on shot difficulty?)
            
            if(Random.Range(0f, 1f) < 0.5f)
            {
                //move backwards, look towards last rail point, play idle animation, move to shot position 

                //move back
                yield return StartSubState(new AISubState(() =>
                {
                    Debug.Log(golfer);
                    Vector2 point = new Vector2(golfer.CurrentTees.BackStandPosition.position.x, golfer.CurrentTees.BackStandPosition.position.z);
                    golfer.StartToMoveToPoint(point);
                }, () => { return golfer.LessThanDistance(0.2f, new Vector2(golfer.CurrentTees.BackStandPosition.position.x, golfer.CurrentTees.BackStandPosition.position.z)); }));
                //look towards last rail point, idle animation
                yield return StartSubState(new AISubState(() =>
                {
                    Debug.Log(points);
                    golfer.StartToLookToPoint(new Vector2(points[points.Count - 1].point.x, points[points.Count - 1].point.z));
                    golfer.AnimationController.SetTrigger("Idle");
                }, () => { return golfer.Turning == false; }));
            }

            yield return new WaitForSeconds(2);

            yield return StartSubState(new AISubState(() =>
            {
                golfer.StartToMoveToPoint(new Vector2(golfer.CurrentTees.RightHandedStandPosition.position.x, golfer.CurrentTees.RightHandedStandPosition.position.z));
            }, () => { return golfer.LessThanDistance(0.1f, new Vector2(golfer.CurrentTees.RightHandedStandPosition.position.x, golfer.CurrentTees.RightHandedStandPosition.position.z)); }));

            yield return StartSubState(new AISubState(() =>
            {
                golfer.StartToLookToPoint(golfer.CurrentTees.CenterPosition);
                golfer.AnimationController.SetTrigger("DefaultToReady");
            }, () => { return golfer.Turning == false; }));

            yield return new WaitForSeconds(3f);

            //in-ready idle animation chance

            yield return StartSubState(new AISubState(() =>
            {
                golfer.AnimationController.SetTrigger("Swing");
            }, () => { return golfer.AEST_SwingBottom(); }));               //Continues when AE_SwingBottom is called by AnimationEvent


            golfer.ballMotion.StartMoveBallOnRail(points, golfer.PlayerBall);

            yield return new WaitForSeconds(1.75f);

            golfer.AnimationController.SetTrigger("SwingToDefault");

            yield return StartSubState(new AISubState(() =>
            {
                golfer.StartToLookToPoint(new Vector2(points[points.Count - 1].point.x, points[points.Count - 1].point.z));
            }, () => { return golfer.Turning == false; }));

            Complete = true;
        }

        public override void OnTickDuringActionIncomplete()
        {
            
        }

        public override void OnFinishedAction()
        {
            Debug.Log("FINISHED");
        }
    }
}
