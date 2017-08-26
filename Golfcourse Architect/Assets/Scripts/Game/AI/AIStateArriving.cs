using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStateArriving : AIState
    {
        public override void OnBecameActiveState()
        {
            golfer.StartToMoveToPoint(golfer.clubhouse.InitialMoveSpot.transform.position); //AIState will handle the completion of movement by default
        }

        public override void OnFinishedAction()
        {
            AIStateWaitingForRound wait = new AIStateWaitingForRound(golfer.clubhouse.InitialMoveSpot.transform.position);
            golfer.State = wait;
        }
    }
}
