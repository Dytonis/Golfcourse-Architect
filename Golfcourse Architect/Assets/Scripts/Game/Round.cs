﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA.Game
{
    [Serializable]
    public struct Round
    {
        public List<int> ScoresIndividual;
        public Dictionary<Golfer, int> OtherScores;

        public TeeTypes TeeType;

        public float TeeTime;

        /// <summary>
        /// The current hole 1 based
        /// </summary>
        public int CurrentHole;

        public bool Started;

        public void Start()
        {
            Started = true;
        }
    }
}
