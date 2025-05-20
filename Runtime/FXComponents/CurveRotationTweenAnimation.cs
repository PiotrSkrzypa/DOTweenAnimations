using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CurveRotationTweenAnimation : BaseFXComponent
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] bool useLocalSpace = true;
        [SerializeField] Vector3 startingRotation;
        [SerializeField] AnimationCurve xCurve;
        [SerializeField] AnimationCurve yCurve;
        [SerializeField] AnimationCurve zCurve;
        [SerializeField] Ease easeType = Ease.Linear;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {

            if (useLocalSpace)
                targetTransform.localEulerAngles = startingRotation;
            else
                targetTransform.eulerAngles = startingRotation;

            await LMotion.Create(0f, 1f, Timing.Duration)
                .WithScheduler(Timing.GetScheduler())
                .WithEase(easeType)
                .Bind(t =>
                {
                    var rot = new Vector3(
                        xCurve?.Evaluate(t) ?? 0f,
                        yCurve?.Evaluate(t) ?? 0f,
                        zCurve?.Evaluate(t) ?? 0f
                    );

                    if (useLocalSpace)
                        targetTransform.localEulerAngles = rot;
                    else
                        targetTransform.eulerAngles = rot;
                })
                .ToUniTask(cancellationToken);
        }

        protected override void StopInternal()
        {
            if (useLocalSpace)
                targetTransform.localEulerAngles = startingRotation;
            else
                targetTransform.eulerAngles = startingRotation;
        }

        protected override void ResetInternal()
        {
            if (useLocalSpace)
                targetTransform.localEulerAngles = startingRotation;
            else
                targetTransform.eulerAngles = startingRotation;
        }
    }
}
