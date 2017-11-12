using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA.Game.GroundTypes
{
    /// <summary>
    /// Tiles set to this will have facades built over them. E.g. cliffs, paths, bunkers, water
    /// </summary>
    public class FacadeBase : GroundType
    {
        public override int[] spriteChildPath
        {
            get
            {
                return new int[]
                {
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.TRANSPARENT,
                };
            }
        }

        public override void OnActivate()
        {
            
        }
    }
}
