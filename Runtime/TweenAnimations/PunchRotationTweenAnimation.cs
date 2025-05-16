using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;
using System.Threading;
using Alchemy.Inspector;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PunchRotationTweenAnimation : ITweenAnimation
    {
        public bool IsRunning => isRunning;
        public float Duration { get => duration; set => duration = value; }
        public int Frequency { get => frequency; set => frequency = value; }
        public float Damping { get => damping; set => damping = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;

        bool isRunning;
        CancellationTokenSource cts;

        [SerializeField] private Transform transformToRotate;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private int frequency = 10;
        [SerializeField] private float delay = 0f;
        [SerializeField] private bool timeScaleIndependent = true;
        [SerializeField] private Vector3 punch;
        Vector3 originalRotation;

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

            isRunning = true;
            preparation?.Invoke();

            cts = new CancellationTokenSource();
            var token = cts.Token;
            var scheduler = timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update;

            if (delay > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: timeScaleIndependent, cancellationToken: token);
            }

            afterDelayCallback?.Invoke();

            originalRotation = useLocalSpace ? transformToRotate.localEulerAngles : transformToRotate.eulerAngles;

            UniTask uniTask = useLocalSpace ? LMotion.Punch.Create(originalRotation, punch, duration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithScheduler(scheduler)
                .Bind(transformToRotate, (v, tr) => transformToRotate.localEulerAngles = v)
                .ToUniTask(token) :
                LMotion.Punch.Create(originalRotation, punch, duration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithScheduler(scheduler)
                .Bind(transformToRotate, (v, tr) => transformToRotate.eulerAngles = v)
                .ToUniTask(token);


            await uniTask;


            isRunning = false;
            callbackAfterAnimation?.Invoke();
        }
        [Button]
        public void Stop()
        {
            if (!isRunning) return;

            isRunning = false;

            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            if (useLocalSpace)
            {
                transformToRotate.localEulerAngles = originalRotation;
            }
            else
            {
                transformToRotate.eulerAngles = originalRotation;
            }
        }
        [Button]
        public void Reset()
        {
            isRunning = false;

            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            if (useLocalSpace)
            {
                transformToRotate.localEulerAngles = originalRotation;
            }
            else
            {
                transformToRotate.eulerAngles = originalRotation;
            }
        }
    }
}
