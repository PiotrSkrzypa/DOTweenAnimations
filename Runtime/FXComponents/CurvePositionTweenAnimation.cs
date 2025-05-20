using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CurvePositionTweenAnimation : BaseFXComponent
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] bool useLocalSpace = true;
        [SerializeField] Vector3 startingPosition;
        [SerializeField] float samplingResolution = 30f;
        [SerializeField] AnimationCurve xPositionCurve;
        [SerializeField] AnimationCurve yPositionCurve;
        [SerializeField] AnimationCurve zPositionCurve;
        [SerializeField] Ease easeType = Ease.Linear;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            if (useLocalSpace)
                targetTransform.localPosition = startingPosition;
            else
                targetTransform.position = startingPosition;

            Vector3[] sampledPath = SampleCurves();

            await LMotion.Create(0f, 1f, Timing.Duration)
                .WithScheduler(Timing.GetScheduler())
                .WithEase(easeType)
                .Bind(t =>
                {
                    int index = Mathf.Clamp(Mathf.RoundToInt(t * (sampledPath.Length - 1)), 0, sampledPath.Length - 1);
                    if (useLocalSpace)
                        targetTransform.localPosition = sampledPath[index];
                    else
                        targetTransform.position = sampledPath[index];
                })
                .ToUniTask(cancellationToken);
        }

        private Vector3[] SampleCurves()
        {
            int steps = Mathf.Max(2, Mathf.RoundToInt(samplingResolution));
            Vector3[] result = new Vector3[steps];
            for (int i = 0; i < steps; i++)
            {
                float t = i / (float)(steps - 1);
                result[i] = new Vector3(
                    xPositionCurve?.Evaluate(t) ?? 0f,
                    yPositionCurve?.Evaluate(t) ?? 0f,
                    zPositionCurve?.Evaluate(t) ?? 0f
                );
            }
            return result;
        }

        protected override void StopInternal()
        {
            if (useLocalSpace)
                targetTransform.localPosition = startingPosition;
            else
                targetTransform.position = startingPosition;
        }

        protected override void ResetInternal()
        {
            if (useLocalSpace)
                targetTransform.localPosition = startingPosition;
            else
                targetTransform.position = startingPosition;
        }
    }
}
