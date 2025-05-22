﻿using System;
using Alchemy.Inspector;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXTiming
    {
        public float InitialDelay;
        public float Duration;
        public float CooldownDuration;
        public bool ContributeToTotalDuration = true;

        public bool TimeScaleIndependent;

        public int NumberOfRepeats;
        public bool RepeatForever;
        public float DelayBetweenRepeats;

        [ReadOnly]public int PlayCount;

        public IMotionScheduler GetScheduler() =>
    TimeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update;
    }
}
