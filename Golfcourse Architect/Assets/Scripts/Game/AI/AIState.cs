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

        public virtual void OnBecameActiveState()
        {

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
    }
}
