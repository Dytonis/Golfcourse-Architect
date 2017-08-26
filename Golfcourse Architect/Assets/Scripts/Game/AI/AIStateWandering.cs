using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIStateWandering : AIState
    {
        public Vector2 wanderCenterPoint;

        public override void OnTickDuringActionIncomplete()
        {
            if (!golfer.Moving)
            {
                if (Random.Range(0f, 1f) < 0.6f)
                {
                    float newX = wanderCenterPoint.x + Random.Range(-4f, 4f);
                    float newY = wanderCenterPoint.x + Random.Range(-4f, 4f);

                    golfer.StartToMoveToPoint(new Vector2(newX, newY));
                }
            }
        }
    }
}
