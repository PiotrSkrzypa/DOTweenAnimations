using System;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class ColorTextMeshProTweenAnimation : ITweenAnimation
    {
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool IsRunning { get; private set; }
        public bool TimeScaleIndependent => timeScaleIndependent;

        [SerializeField] TMP_Text textToColor;
        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;

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

            float animationDelay = (textToColor.color == startColor) ? delay : 0f;

            if (animationDelay > 0)
                await UniTask.Delay(TimeSpan.FromSeconds(animationDelay), ignoreTimeScale: timeScaleIndependent, cancellationToken: cancellationTokenSource.Token);

            afterDelayCallback?.Invoke();

            var handle = LMotion.Create(startColor, targetColor, duration)
                .WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .BindToColor(textToColor);

            await handle.ToUniTask(cancellationTokenSource.Token);

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
                textToColor.color = startColor;
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

            textToColor.color = startColor;
        }
    }
}
