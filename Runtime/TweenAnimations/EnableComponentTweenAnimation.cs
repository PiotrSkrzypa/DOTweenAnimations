using System;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class EnableComponentTweenAnimation : ITweenAnimation
    {
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool IsRunning => isRunning;
        public bool TimeScaleIndependent => timeScaleIndependent;

        bool isRunning;

        [SerializeField] Behaviour componentToEnable;
        [SerializeField] float duration = 0.2f;
        [SerializeField] float delay = 0f;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] bool targetState = true;

        bool originalState;

        CancellationTokenSource cancellationTokenSource;

        #region Callbacks
        TweenAnimationCallback preparation;
        TweenAnimationCallback afterDelayCallback;
        TweenAnimationCallback callbackAfterAnimation;

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

        async UniTaskVoid PlayAsync()
        {
            if (componentToEnable == null)
            {
                Debug.LogWarning("[EnableComponentTweenAnimation] componentToEnable is null.");
                return;
            }

            isRunning = true;

            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            preparation?.Invoke();

            originalState = componentToEnable.enabled;

            if (Delay > 0f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(Delay),
                    ignoreTimeScale: timeScaleIndependent, 
                    cancellationToken: cancellationTokenSource.Token);
            }

            componentToEnable.enabled = targetState;
            afterDelayCallback?.Invoke();

            if (Duration > 0f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(Duration),
                    ignoreTimeScale: timeScaleIndependent, 
                    cancellationToken: cancellationTokenSource.Token);
            }

            isRunning = false;
            callbackAfterAnimation?.Invoke();
        }
        [Button]
        public void Stop()
        {
            if (!isRunning) return;
            isRunning = false;

            if (componentToEnable != null)
            {
                componentToEnable.enabled = originalState;
            }
            cancellationTokenSource?.Cancel();
        }
        [Button]
        public void Reset()
        {
            isRunning = false;

            if (componentToEnable != null)
            {
                componentToEnable.enabled = originalState;
            }
            cancellationTokenSource?.Cancel();
        }
    }
}
