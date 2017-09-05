using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA
{
    public class WaitWhile : IEnumerator
    {
        private Func<bool> predicate;

        public object Current
        {
            get
            {
                return null;
            }
        }

        public bool MoveNext()
        {
            return predicate();
        }

        public void Reset()
        {
            
        }

        public WaitWhile(Func<bool> predicate)
        {
            this.predicate = predicate;
        }
    }
}
