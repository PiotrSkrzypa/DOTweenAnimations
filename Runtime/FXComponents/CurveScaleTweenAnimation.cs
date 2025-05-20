using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CurveScaleTweenAnimation : BaseFXComponent
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] Vector3 startingScale = Vector3.one;
        [SerializeField] AnimationCurve xCurve;
        [SerializeField] AnimationCurve yCurve;
        [SerializeField] AnimationCurve zCurve;
        [SerializeField] Ease easeType = Ease.Linear;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            targetTransform.localScale = startingScale;

            await LMotion.Create(0f, 1f, Timing.Duration)
                .WithScheduler(Timing.GetScheduler())
                .WithEase(easeType)
                .Bind(t =>
                {
                    var scale = new Vector3(
                        xCurve?.Evaluate(t) ?? 1f,
                        yCurve?.Evaluate(t) ?? 1f,
                        zCurve?.Evaluate(t) ?? 1f
                    );
                    targetTransform.localScale = scale;
                })
                .ToUniTask(cancellationToken);
        }

        protected override void StopInternal()
        {
            targetTransform.localScale = startingScale;
        }

        protected override void ResetInternal()
        {
            targetTransform.localScale = startingScale;
        }
    }
}
