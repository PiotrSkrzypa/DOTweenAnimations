using System;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CurveScaleTweenAnimation : ITweenAnimation
    {
        public bool IsRunning { get; private set; }
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;

        [SerializeField] Transform targetTransform;
        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] Vector3 startingScale = Vector3.one;
        [SerializeField] AnimationCurve xCurve;
        [SerializeField] AnimationCurve yCurve;
        [SerializeField] AnimationCurve zCurve;
        [SerializeField] Ease easeType = Ease.Linear;

        TweenAnimationCallback preparation;
        TweenAnimationCallback afterDelayCallback;
        TweenAnimationCallback callbackAfterAnimation;

        CancellationTokenSource cancellationTokenSource;

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
            targetTransform.localScale = startingScale;

            if (delay > 0)
                await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: timeScaleIndependent, cancellationToken: cancellationTokenSource.Token);

            afterDelayCallback?.Invoke();

            await LMotion.Create(0f, 1f, duration)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
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
                .ToUniTask(cancellationTokenSource.Token);

            IsRunning = false;
            callbackAfterAnimation?.Invoke();
        }

        [Button]
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                cancellationTokenSource?.Cancel();
                targetTransform.localScale = startingScale;
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

            targetTransform.localScale = startingScale;
        }
    }
}
