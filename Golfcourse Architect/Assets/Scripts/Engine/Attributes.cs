using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA.Engine
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TileObjectEffectInverseColorAttribute : Attribute
    {
        private bool b;

        public TileObjectEffectInverseColorAttribute(bool b)
        {
            this.b = b;
        }

        public bool Inverse
        {
            get
            {
                return b;
            }
            set
            {
                b = value;
            }
        }
    }
}
