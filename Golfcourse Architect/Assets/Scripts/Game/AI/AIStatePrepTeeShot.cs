using GA.Pathfinding;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStatePrepShot : AIState
    {
        private BallPathBlock block;

        public override void OnBecameActiveState()
        {
            ShotPoint nextShotPoint = golfer.CurrentHole.Golfer_CalculateNextShot(golfer, golfer.CurrentTees.FlatPosition);
            block = golfer.PlayerBall.GetComponent<BallPathFinder>().FindPathFlatTarget(golfer.PlayerBall.transform.position, nextShotPoint.point); //find the shot to the next point

            Complete = true;
        }

        public override void OnFinishedAction()
        {
            golfer.State = new AIStateHitTeeShot(block.rail);
        }

        public override void OnTickDuringActionIncomplete()
        {
            
        }
    }
}
