using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStateHitShot : AIState
    {
        ShotPath path;

        public AIStateHitShot(ShotPath targetLocation)
        {
            path = targetLocation;
        }

        public override void OnBecameActiveState()
        {
            BallPathSolver solver = golfer.solver;
            solver.target = new Vector3(path.points[1].x, 0, path.points[1].y);
            solver.ball = golfer.PlayerBall;
            solver.Level = 999;
            solver.startingPos = new Vector3(golfer.FlatPosition.x, 0, golfer.FlatPosition.y);
            solver.StartAttempToFindStandard();
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
