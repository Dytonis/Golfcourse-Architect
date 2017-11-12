using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA.Game.GroundTypes
{
    /// <summary>
    /// Similar to fairway, but has a 0 shotFromRiskPenalty and contains the Tees object it is bound to.
    /// </summary>
    public class Teebox : Fairway
    {
        public override float shotFromRiskPenalty
        {
            get
            {
                return 0;
            }

            set
            {
                base.shotFromRiskPenalty = value;
            }
        }

        public Tees TeesBoundTo;
    }
}
