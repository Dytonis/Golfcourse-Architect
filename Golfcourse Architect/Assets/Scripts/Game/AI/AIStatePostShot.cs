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
        public override IEnumerator EnumerationOnBecameActiveState()
        {
            yield return new WaitWhile(() => { return golfer.PlayerBall.Velocity.magnitude > 0.3f; });

            yield return StartSubState(new AISubState(() =>
            {
                golfer.StartToPathToPoint(golfer.PlayerBall.FlatPosition, golfer.family); //check if it's possible here!
            }, () => { return golfer.FinishedMoving == true; }));

            Complete = true;
        }

        public override void OnTickDuringActionIncomplete()
        {
            
        }

        public override void OnFinishedAction()
        {
            Debug.Log("LOG");

            golfer.State = new AIStatePrepShot(golfer.PlayerBall.FlatPosition); 
        }
    }
}
