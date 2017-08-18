using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStateWaitingForRound : AIState
    {
        private Vector2 wanderCenterPoint;

        public AIStateWaitingForRound(Vector2 wanderPoint)
        {
            wanderCenterPoint = wanderPoint;
        }

        public override void OnTickDuringActionIncomplete()
        {
            if (golfer.gamemode.TimeOfDay >= golfer.round.TeeTime - 100)
            {
                if (golfer.GroupSize < 2 || golfer.GroupHonors == true)
                {
                    golfer.round.CurrentHole = 1;
                    golfer.round.Start();
                    golfer.State = new AI.AIStateTeeBall();
                }
            }

            //wander, sit, eat, talk, drink, buy
            if (Random.Range(0f, 1f) < 0.5f)
            {
                if (!golfer.Moving)
                {
                    float newX = wanderCenterPoint.x + Random.Range(-4f, 4f);
                    float newY = wanderCenterPoint.x + Random.Range(-4f, 4f);

                    golfer.StartToMovePoint(new Vector2(newX, newY));
                }
            }
            else
            {
                //look for a place to sit
            }
        }
    }
}
