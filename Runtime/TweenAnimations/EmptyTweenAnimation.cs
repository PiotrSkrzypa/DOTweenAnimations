using System;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using LitMotion;
using System.Threading;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class EmptyTweenAnimation : ITweenAnimation
    {
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;
        public bool IsRunning { get; private set; }

        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;

        CancellationTokenSource cancellationTokenSource;

        #region Callbacks
        public TweenAnimationCallback BeforeAnimationCallback => preparation;

        public TweenAnimationCallback AfterDelayCallback => afterDelayCallback;

        public TweenAnimationCallback AfterAnimationCallback => callbackAfterAnimation;


        TweenAnimationCallback preparation;
        TweenAnimationCallback afterDelayCallback;
        TweenAnimationCallback callbackAfterAnimation;
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

            if (delay > 0)
                await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: timeScaleIndependent, cancellationToken: cancellationTokenSource.Token);

            afterDelayCallback?.Invoke();

            if (duration > 0)
                await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: timeScaleIndependent, cancellationToken: cancellationTokenSource.Token);

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
        }
    }
}