using GA.Game.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA
{
    public class WaitForSubStateComplete : IEnumerator
    {
        private AISubState state;

        public object Current
        {
            get
            {
                return null;
            }
        }

        public WaitForSubStateComplete(AISubState state)
        {
            this.state = state;
        }

        public bool MoveNext()
        {
            return (!state.Complete);
        }

        public void Reset() { }
    }
}
