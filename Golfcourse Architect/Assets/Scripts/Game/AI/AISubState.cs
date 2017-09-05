using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    /// <summary>
    /// Sealed AISubState intended on implementing actions such as moving an actor to a location, or starting an event, and maintaining AI execution order. Must be called via IEnumerator AIState.StartSubState(..)
    /// </summary>
    public sealed class AISubState
    { 
        private Action routine;
        private Func<bool> completion;

        public bool Complete;

        public AISubState(Action action, Func<bool> completion)
        {
            routine = action;
            this.completion = completion;
        }

        public bool GetComplete()
        {
            if (completion != null)
                return completion();
            else return Complete;
        }

        public void Start()
        {
            if (routine != null)
                routine.Invoke();
        }
    }
}
