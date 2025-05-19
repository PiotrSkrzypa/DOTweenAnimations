using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class LightTweenAnimation : BaseFXComponent
    {
        [SerializeField] Light light;
        [SerializeField] float startIntensity = 0f;
        [SerializeField] float targetIntensity = 1f;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            if (light == null)
            {
                Debug.LogWarning("[LightTweenAnimation] Light reference is null.");
                return;
            }

            light.intensity = startIntensity;

            float elapsed = 0f;

            while (elapsed < Timing.Duration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                float t = elapsed / Timing.Duration;
                light.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                await UniTask.Yield(PlayerLoopTiming.Update);

                elapsed += Timing.TimeScaleIndependent ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            light.intensity = targetIntensity;
        }

        protected override void StopInternal()
        {
            if (light != null)
                light.intensity = startIntensity;
        }

        protected override void ResetInternal()
        {
            if (light != null)
                light.intensity = startIntensity;
        }
    }
}
