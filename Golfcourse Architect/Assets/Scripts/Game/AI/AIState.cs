using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Game.AI
{
    public class AIState
    {
        private AISubState subState;

        public Golfer golfer;
        private bool _complete;
        public bool Complete
        {
            get
            {
                return _complete;
            }
            set
            {
                OnFinishedAction();
                _complete = value;
            }
        }
        public float TimeComplete;

        protected byte AnimationIndex = 0;

        protected WaitForSubStateComplete StartSubState(AISubState state)
        {
            subState = null;
            subState = state;
            state.Start();
            return new WaitForSubStateComplete(state);
        }

        public virtual void OnBecameActiveState()
        {

        }

        public virtual IEnumerator EnumerationOnBecameActiveState()
        {
            yield break;
        }

        public virtual void OnTickDuringActionIncomplete()
        {
            if (!golfer.Moving)
                Complete = true;
        }

        public virtual void OnTickDuringActionComplete()
        {

        }

        public virtual void OnFinishedAction()
        {

        }

        public virtual void CheckForSubStateCompletion()
        {
            if(subState != null)
            {
                subState.Complete = subState.GetComplete();
            }
        }
    }
}
