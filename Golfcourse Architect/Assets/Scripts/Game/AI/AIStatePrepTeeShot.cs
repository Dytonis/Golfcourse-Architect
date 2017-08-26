using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStatePrepShot : AIState
    {
        private ShotPath path;

        public override void OnBecameActiveState()
        {
            path = golfer.CurrentHole.Golfer_CalculateShotPosition(golfer, golfer.CurrentTees.FlatPosition);
            golfer.StartToMoveToPoint(new Vector2(golfer.FlatPosition.x - 1, golfer.FlatPosition.y), false);    
        }

        public override void OnFinishedAction()
        {
            golfer.State = new AI.AIStateHitShot(path);
        }
    }
}
