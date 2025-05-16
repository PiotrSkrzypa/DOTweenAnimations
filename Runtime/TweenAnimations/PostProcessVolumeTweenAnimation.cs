using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using LitMotion;
using LitMotion.Extensions;
using Alchemy.Inspector;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PostProcessVolumeTweenAnimation : ITweenAnimation
    {
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;
        public bool IsRunning => isRunning;

        bool isRunning;
        CancellationTokenSource cts;

        [SerializeField] private Volume volume;
        [SerializeField] private float duration;
        [SerializeField] private float delay;
        [SerializeField] private bool timeScaleIndependent = true;
        [SerializeField] private float startWeight;
        [SerializeField] private float targetWeight;

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

        private async UniTaskVoid PlayAsync()
        {
            Stop();

            if (volume == null) return;

            isRunning = true;
            preparation?.Invoke();

            cts = new CancellationTokenSource();
            var token = cts.Token;

            volume.weight = startWeight;

            if (delay > 0)
                await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: timeScaleIndependent, cancellationToken: token);

            afterDelayCallback?.Invoke();

            var scheduler = timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update;

            try
            {
                var handle = LMotion.Create(startWeight, targetWeight, duration)
                    .WithEase(Ease.OutQuad)
                    .WithScheduler(scheduler)
                    .Bind(volume, (x, v) => v.weight = x);

                await handle.ToUniTask(token);
            }
            catch (OperationCanceledException) { }

            isRunning = false;
            callbackAfterAnimation?.Invoke();
        }
        [Button]
        public void Stop()
        {
            if (!isRunning) return;

            isRunning = false;
            volume.weight = startWeight;

            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }
        [Button]
        public void Reset()
        {
            Stop();
            volume.weight = startWeight;
        }
    }
}
