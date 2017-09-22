using GA.Physics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using GA.Pathfinding.Ballfinding;

namespace GA.Game.AI
{
    public class AIStateHitShot : AIState
    {
        BallPathBlock block;
        List<RailPoint> points;

        public AIStateHitShot(BallPathBlock rail)
        {
            block = rail;
            points = block.rail.ToList();
        }

        public override IEnumerator EnumerationOnBecameActiveState()
        {
            //chance to move backwards and look (maybe scale that based on shot difficulty?)

            Vector3 direction = -Math.Direction(golfer.PlayerBall.transform.position, points.Last().point).normalized;
            Vector2 backupPoint = (golfer.PlayerBall.transform.position + (direction * 0.2f)).ToVector2();
            Vector3 perpDirection = Vector3.Cross(Vector3.up, direction); //get the perpendicular direction
            Vector2 rightPoint = (golfer.PlayerBall.transform.position + (perpDirection * 0.2f)).ToVector2();

            if (Random.Range(0f, 1f) < 0.5f)
            {
                //move backwards, look towards last rail point, play idle animation, move to shot position 

                //move back
                yield return StartSubState(new AISubState(() =>
                {
                    Debug.Log(golfer);
                    golfer.StartToMoveToPoint(backupPoint);
                }, () => { return golfer.LessThanDistance(0.2f, backupPoint); }));
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
                golfer.StartToMoveToPoint(rightPoint);
            }, () => { return golfer.LessThanDistance(0.1f, rightPoint); }));

            yield return StartSubState(new AISubState(() =>
            {
                golfer.StartToLookToPoint(golfer.PlayerBall.transform.position.ToVector2());
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
            yield break;
        }

        public override void OnTickDuringActionIncomplete()
        {
            
        }

        public override void OnFinishedAction()
        {
            golfer.State = new AIStatePostShot(block);
            Debug.Log("FINISHED");
        }
    }
}
