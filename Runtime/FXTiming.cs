using System;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public  class FXTiming
    {
        public float InitialDelay;
        public float Duration;
        public float CooldownDuration;
        public bool ContributeToTotalDuration = true;

        public bool TimeScaleIndependent;

        public int NumberOfRepeats;
        public bool RepearForever;
        public float DelayBetweenRepeats;

        public int PlayCount;

        public bool IsRunning { get; set; }
    }
}
