using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using Alchemy.Inspector;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PositionTweenAnimation : ITweenAnimation
    {
        public bool IsRunning => isRunning;
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;

        bool isRunning;
        CancellationTokenSource cts;

        [SerializeField] private Transform transformToMove;
        [SerializeField] private float duration;
        [SerializeField] private float delay;
        [SerializeField] bool useLocalSpace = true;
        [SerializeField] private bool timeScaleIndependent = true;
        [SerializeField] private Vector3 startingPosition;
        [SerializeField] private Vector3 targetPosition;

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

            if (transformToMove == null)
                return;

            isRunning = true;
            preparation?.Invoke();

            cts = new CancellationTokenSource();
            var token = cts.Token;

            if (delay > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: timeScaleIndependent, cancellationToken: token);
            }

            afterDelayCallback?.Invoke();

            ResetPosition();

            var scheduler = timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update;


            var handle = useLocalSpace ? LMotion.Create(startingPosition, targetPosition, duration)
                    .WithEase(Ease.OutQuad)
                    .WithScheduler(scheduler)
                    .Bind(transformToMove, (x, t) => t.localPosition = x) :
                    LMotion.Create(startingPosition, targetPosition, duration)
                    .WithEase(Ease.OutQuad)
                    .WithScheduler(scheduler)
                    .Bind(transformToMove, (x, t) => t.position = x);

            await handle.ToUniTask(cts.Token);

            isRunning = false;
            callbackAfterAnimation?.Invoke();
        }
        private void ResetPosition()
        {
            if (useLocalSpace)
            {
                transformToMove.localPosition = startingPosition;
            }
            else
            {
                transformToMove.position = startingPosition;
            }
        }
        [Button]
        public void Stop()
        {
            if (!isRunning) return;

            isRunning = false;
            cts?.Cancel();
            cts?.Dispose();
            cts = null;

            ResetPosition();
        }
        [Button]
        public void Reset()
        {
            isRunning = false;
            cts?.Cancel();
            cts?.Dispose();
            cts = null;

            ResetPosition();
        }

    }
}
