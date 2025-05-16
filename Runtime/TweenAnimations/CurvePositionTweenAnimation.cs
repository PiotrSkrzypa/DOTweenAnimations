using System;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CurvePositionTweenAnimation : ITweenAnimation
    {
        public bool IsRunning { get; private set; }
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;

        [SerializeField] Transform targetTransform;
        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] bool useLocalSpace = true;
        [SerializeField] Vector3 startingPosition;
        [SerializeField] float samplingResolution = 30f;
        [SerializeField] AnimationCurve xPositionCurve;
        [SerializeField] AnimationCurve yPositionCurve;
        [SerializeField] AnimationCurve zPositionCurve;
        [SerializeField] Ease easeType = Ease.Linear;

        TweenAnimationCallback preparation;
        TweenAnimationCallback afterDelayCallback;
        TweenAnimationCallback callbackAfterAnimation;

        CancellationTokenSource cancellationTokenSource;

        #region Callbacks
        public TweenAnimationCallback BeforeAnimationCallback => preparation;
        public TweenAnimationCallback AfterDelayCallback => afterDelayCallback;
        public TweenAnimationCallback AfterAnimationCallback => callbackAfterAnimation;

        public ITweenAnimation WithBeforeAnimationCallback(TweenAnimationCallback beforeAnimationCallback)
        {
            preparation = beforeAnimationCallback;
            return this;
        }

        public ITweenAnimation WithAfterDelayCallback(TweenAnimationCallback afterDelayCallback)
        {
            this.afterDelayCallback = afterDelayCallback;
            return this;
        }

        public ITweenAnimation WithAfterAnimationCallback(TweenAnimationCallback afterAnimationCallback)
        {
            callbackAfterAnimation = afterAnimationCallback;
            return this;
        }
        #endregion

        [Button]
        public void Play()
        {
            _ = PlayAsync();
        }

        private async UniTask PlayAsync()
        {
            IsRunning = true;
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            preparation?.Invoke();

            if (useLocalSpace)
                targetTransform.localPosition = startingPosition;
            else
                targetTransform.position = startingPosition;

            Vector3[] sampledPath = SampleCurves();

            if (delay > 0)
                await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: timeScaleIndependent, cancellationToken: cancellationTokenSource.Token);

            afterDelayCallback?.Invoke();

            await LMotion.Create(0f, 1f, duration)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .WithEase(easeType)
                .Bind(t =>
                {
                    int index = Mathf.Clamp(Mathf.RoundToInt(t * (sampledPath.Length - 1)), 0, sampledPath.Length - 1);
                    if (useLocalSpace)
                        targetTransform.localPosition = sampledPath[index];
                    else
                        targetTransform.position = sampledPath[index];
                })
                .ToUniTask(cancellationTokenSource.Token);

            IsRunning = false;
            callbackAfterAnimation?.Invoke();
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

        [Button]
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                cancellationTokenSource?.Cancel();
                if (useLocalSpace)
                    targetTransform.localPosition = startingPosition;
                else
                    targetTransform.position = startingPosition;
            }
        }

        [Button]
        public void Reset()
        {
            if (IsRunning)
            {
                IsRunning = false;
                cancellationTokenSource?.Cancel();
            }

            if (useLocalSpace)
                targetTransform.localPosition = startingPosition;
            else
                targetTransform.position = startingPosition;
        }
    }
}
