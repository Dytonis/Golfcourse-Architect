using System;

namespace GA.Game.GroundTypes
{
    public class Gravel : GroundType
    {
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

        public override float shotFromRiskPenalty
        {
            get
            {
                return 3;
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
                return 15;
            }

            set
            {
                base.shotRisk = value;
            }
        }

        public override float friction
        {
            get
            {
                return 0.1f;
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
                return 0.95f;
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

        public override int[] spriteChildPath
        {
            get
            {
                return new int[]
                {
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.GRAVEL,
                    (int)GroundSprites.GRAVEL,
                };
            }
        }

        public override void OnActivate()
        {
            throw new NotImplementedException();
        }
    }
}
