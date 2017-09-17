using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStateTeeBall : AIState
    {
        public bool AtTeeBox = false;

        public override void OnBecameActiveState()
        {
            if (golfer.gamemode.HoleList.Count >= golfer.round.CurrentHole - 1)
            {
                Hole goToHole = golfer.gamemode.HoleList[golfer.round.CurrentHole - 1];

                if (goToHole.Open)
                {
                    golfer.StartToPathToPoint(new Vector2(goToHole.TeesList[0].Position.x, goToHole.TeesList[0].Position.z), golfer.family);
                }
                else
                {
                    //get the next open hole
                }
            }
        }

        public override void OnTickDuringActionIncomplete()
        {
            if(!golfer.Moving && AnimationIndex == 0)
            {
                AnimationIndex = 1;
                golfer.AnimationController.SetTrigger("Tee_Ball");
                golfer.CompleteInSeconds(1);
            }
        }

        public override void OnFinishedAction()
        {
            golfer.State = new AI.AIStatePrepShot(golfer.PlayerBall.FlatPosition);
        }
    }
}
