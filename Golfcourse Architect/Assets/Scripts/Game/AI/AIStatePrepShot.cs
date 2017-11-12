using GA.Pathfinding;
using GA.Pathfinding.Ballfinding;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStatePrepShot : AIState
    {
        private BallPathBlock block;
        private Vector2 origin;

        public AIStatePrepShot(Vector2 start)
        {
            origin = start;
        }

        public override void OnBecameActiveState()
        {
            ShotPoint nextShotPoint = golfer.CurrentHole.Golfer_CalculateNextShot(golfer, origin);

            if (golfer.family.GetChunkDataPointGroundTypeGlobally(golfer.PlayerBall.FlatPosition) is GA.Game.GroundTypes.Green)
            {
                block = golfer.PlayerBall.GetComponent<BallPathFinder>().FindPathFlatTarget_Putt(golfer.PlayerBall.transform.position, nextShotPoint.point);
            }
            else
            {              
                block = golfer.PlayerBall.GetComponent<BallPathFinder>().FindPathFlatTarget_Standard(golfer.PlayerBall.transform.position, nextShotPoint.point); //find the shot to the next point
            }

            Complete = true;
        }

        public override void OnFinishedAction()
        {
            golfer.State = new AIStateHitShot(block);
        }

        public override void OnTickDuringActionIncomplete()
        {
            
        }
    }
}
