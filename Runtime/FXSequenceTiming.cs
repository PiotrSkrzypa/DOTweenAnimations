using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXSequenceTiming : FXTiming
    {
        public float ComputedDuration { get; private set; }

        public void RecalculateDuration(IFXComponent[] components, SequencePlayMode mode)
        {
            float duration = 0f;

            foreach (var comp in components)
            {
                var timing = comp.Timing;
                if (timing == null) continue;

                float compDuration = timing.InitialDelay + timing.Duration;

                if (timing.RepeatForever)
                {
                    // Impossible to compute, fallback to 1 cycle
                    compDuration += timing.DelayBetweenRepeats;
                }
                else
                {
                    compDuration = ( compDuration + timing.DelayBetweenRepeats ) * Mathf.Max(1, timing.NumberOfRepeats);
                }

                switch (mode)
                {
                    case SequencePlayMode.Parallel:
                        duration = Mathf.Max(duration, compDuration);
                        break;
                    case SequencePlayMode.Sequential:
                        duration += compDuration;
                        break;
                }
            }

            if (RepeatForever)
            {
                ComputedDuration = float.PositiveInfinity;
                return;
            }

            float loop = (duration + DelayBetweenRepeats) * Mathf.Max(1, NumberOfRepeats);
            ComputedDuration = InitialDelay + loop + CooldownDuration;
        }
    } 
}
