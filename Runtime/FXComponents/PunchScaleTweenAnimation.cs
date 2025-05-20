using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PunchScaleTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Transform transformToScale;
        [SerializeField] private Vector3 punch = Vector3.one * 0.5f;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private int frequency = 10;

        private Vector3 originalScale;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            if (transformToScale == null)
            {
                Debug.LogWarning("[PunchScaleTweenAnimation] Transform to scale is null.");
                return;
            }

            originalScale = transformToScale.localScale;

            var scheduler = Timing.GetScheduler();

            try
            {
                var punchTween = LMotion.Punch.Create(originalScale, punch, Timing.Duration)
                    .WithFrequency(frequency)
                    .WithDampingRatio(damping)
                    .WithScheduler(scheduler)
                    .Bind(transformToScale, (v, t) => t.localScale = v);

                await punchTween.ToUniTask(cancellationToken);
            }
            catch (OperationCanceledException) { }
        }

        protected override void StopInternal()
        {
            if (transformToScale != null)
                transformToScale.localScale = originalScale;
        }

        protected override void ResetInternal()
        {
            if (transformToScale != null)
                transformToScale.localScale = originalScale;
        }
    }
}
