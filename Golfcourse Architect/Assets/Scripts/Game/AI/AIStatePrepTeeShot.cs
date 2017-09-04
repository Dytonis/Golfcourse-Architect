using GA.Pathfinding;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStatePrepShot : AIState
    {
        private ShotPoint nextShotPoint;

        public override void OnBecameActiveState()
        {
            nextShotPoint = golfer.CurrentHole.Golfer_CalculateNextShot(golfer, golfer.CurrentTees.FlatPosition);
            BallPathBlock block = golfer.PlayerBall.GetComponent<BallPathFinder>().FindPathFlatTarget(golfer.PlayerBall.transform.position, nextShotPoint.point); //find the shot to the next point

            Vector3 vel = block.velocity;

            golfer.physics.StartMoveBallOnRail(block.rail, golfer.PlayerBall);

            golfer.StartToMoveToPoint(new Vector2(golfer.FlatPosition.x - 1, golfer.FlatPosition.y), false);
        }

        public override void OnFinishedAction()
        {
            
        }
    }
}
