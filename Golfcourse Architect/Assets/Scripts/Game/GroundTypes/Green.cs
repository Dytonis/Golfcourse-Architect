using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA.Game.GroundTypes
{
    /// <summary>
    /// Smooth and bouncy for grass. Can have a hole placed on it.
    /// </summary>
    public class Green : GroundType
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

        public override float shotRisk
        {
            get
            {
                return 0;
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
                return 0.9f;
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
                return 0.35f;
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
                return 0;
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
                return 2;
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
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.GREEN_STANDARD,
                };
            }
        }

        public override void OnActivate()
        {
            throw new NotImplementedException();
        }
    }
}
