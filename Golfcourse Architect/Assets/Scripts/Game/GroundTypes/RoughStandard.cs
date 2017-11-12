using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA.Game.GroundTypes
{
    public class Rough_Standard : GroundType
    {
        public override int[] spriteChildPath
        {
            get
            {
                return new int[]
                {
                    (int)GroundSprites.ROUGH_STANDARD_MT,
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.DARK_PLATE,
                };
            }
        }

        public override float shotFromRiskPenalty
        {
            get
            {
                return 2;
            }

            set
            {
                base.shotFromRiskPenalty = value;
            }
        }

        public override float shotRisk
        {
            get
            {
                return 5;
            }

            set
            {
                base.shotRisk = value;
            }
        }

        public override bool walkable
        {
            get
            {
                return true;
            }

            set
            {
                base.walkable = value;
            }
        }

        public override float friction
        {
            get
            {
                return 0.45f;
            }

            set
            {
                base.friction = value;
            }
        }

        public override float restitution
        {
            get
            {
                return 0.15f;
            }

            set
            {
                base.restitution = value;
            }
        }

        public override float shotWeight
        {
            get
            {
                return 3;
            }

            set
            {
                base.shotWeight = value;
            }
        }

        public override float walkWeight
        {
            get
            {
                return 1;
            }

            set
            {
                base.walkWeight = value;
            }
        }

        public override void OnActivate()
        {
            throw new NotImplementedException();
        }
    }
}
