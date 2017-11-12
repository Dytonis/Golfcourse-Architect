using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA.Game.GroundTypes
{
    /// <summary>
    /// Assigned to vertices that aren't assigned to a tile (eg the last vertex in the x or y lists). Doesn't do anything and will never be interacted with (hopefully)
    /// </summary>
    public class Max_Bounds : GroundType
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
            return;
        }
    }
}
