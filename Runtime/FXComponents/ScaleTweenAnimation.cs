using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;
using System.Threading;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class ScaleTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Transform transformToScale;
        [SerializeField] private Vector3 targetScale;
        Vector3 originalScale;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            var scheduler = Timing.GetScheduler();

            originalScale = transformToScale.localScale;

            UniTask uniTask = LMotion.Create(originalScale, targetScale, Timing.Duration)
                .WithScheduler(scheduler)
                .Bind(transformToScale, (v, tr) => transformToScale.localScale = v)
                .ToUniTask(cancellationToken);

            await uniTask;
        }

        protected override void StopInternal()
        {
            transformToScale.localScale = originalScale;
        }

        protected override void ResetInternal()
        {
            transformToScale.localScale = originalScale;
        }
    }
}
