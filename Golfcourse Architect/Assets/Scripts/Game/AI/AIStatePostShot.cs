using GA.Pathfinding.Ballfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStatePostShot : AIState
    {
        private BallPathBlock ShotBlock;

        public AIStatePostShot(BallPathBlock block)
        {
            ShotBlock = block;
        }

        public override IEnumerator EnumerationOnBecameActiveState()
        {
            yield return new WaitWhile(() => { return golfer.PlayerBall.Velocity.magnitude > 0.3f; });

            yield return StartSubState(new AISubState(() =>
            {
                golfer.StartToPathToPoint(golfer.PlayerBall.FlatPosition, golfer.family); //check if it's possible here!
            }, () => { return golfer.FinishedPathing == true; }));

            if(ShotBlock.isEndingInHole)
            {
                //next hole if there is one
                golfer.AnimationController.SetTrigger("ReachDown");

                yield return new WaitForSeconds(1.5f);

                if (golfer.IsNextHole)
                {
                    golfer.round.CurrentHole++;

                    golfer.State = new AIStateTeeBall();
                    yield break;
                }
                else
                {
                    yield return StartSubState(new AISubState(() =>
                    {
                        golfer.StartToPathToPoint(golfer.clubhouse.FlatPosition, golfer.family);
                    }, () => { return golfer.FinishedPathing == true; }));

                    golfer.State = new AIStateIdle();
                    yield break;
                }
            }

            Complete = true;
            yield break;
        }

        public override void OnTickDuringActionIncomplete()
        {
            
        }

        public override void OnFinishedAction()
        {
            golfer.State = new AIStatePrepShot(golfer.PlayerBall.FlatPosition);
        }
    }
}
