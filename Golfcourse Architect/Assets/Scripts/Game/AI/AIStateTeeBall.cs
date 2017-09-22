using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStateTeeBall : AIState
    {
        public bool AtTeeBox = false;

        public override IEnumerator EnumerationOnBecameActiveState()
        {
            if (golfer.CurrentHole)
            {
                if (golfer.CurrentHole.Open)
                {
                    yield return StartSubState(new AISubState(() =>
                    {
                        golfer.StartToPathToPoint(golfer.CurrentTees.FlatPosition, golfer.family);
                    }, () => { return golfer.FinishedPathing == true; }));

                    yield return StartSubState(new AISubState(() =>
                    {
                        golfer.AnimationController.SetTrigger("ReachDown");
                    }, () => { return golfer.AEST_ReachDown(); }));

                    golfer.SpawnBall(golfer.CurrentTees.BallSpawnPosition, golfer.CurrentTees.transform.rotation, true);

                    yield return new WaitForSeconds(0.15f);

                    Complete = true;
                    yield break;
                }
                else
                {
                    //get the next open hole
                }
            }
        }

        public override void OnTickDuringActionIncomplete()
        {
            
        }

        public override void OnFinishedAction()
        {
            golfer.State = new AI.AIStatePrepShot(golfer.PlayerBall.FlatPosition);
        }
    }
}
