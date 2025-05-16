using System;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class GradientColorGraphicTweenAnimation : ITweenAnimation
    {
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;
        public bool IsRunning => isRunning;

        bool isRunning;

        [SerializeField] Graphic graphicToColor;
        [SerializeField] float duration = 1f;
        [SerializeField] float delay = 0f;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] float gradientSamplingResolution = 30f;
        [SerializeField] Gradient gradient;

        Color[] colorSamples;
        Color originalColor;

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

        private async UniTask PlayAsync()
        {
            if (graphicToColor == null)
            {
                Debug.LogWarning("[GradientColorGraphicTweenAnimation] graphicToColor is null.");
                return;
            }

            isRunning = true;

            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            preparation?.Invoke();
            originalColor = graphicToColor.color;

            if (delay > 0f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), 
                    ignoreTimeScale: timeScaleIndependent,
                    cancellationToken: cancellationTokenSource.Token);
            }

            afterDelayCallback?.Invoke();

            int steps = Mathf.Max(1, Mathf.RoundToInt(gradientSamplingResolution));
            float stepDuration = duration / steps;
            colorSamples = new Color[steps];

            for (int i = 0; i < steps; i++)
            {
                float t = i / (steps - 1f);
                colorSamples[i] = gradient.Evaluate(t);
            }

            for (int i = 0; i < colorSamples.Length; i++)
            {
                graphicToColor.color = colorSamples[i];
                await UniTask.Delay(TimeSpan.FromSeconds(stepDuration), 
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

            if (graphicToColor != null)
            {
                graphicToColor.color = originalColor;
            }
            cancellationTokenSource?.Cancel();
        }
        [Button]
        public void Reset()
        {
            if (graphicToColor != null)
            {
                graphicToColor.color = originalColor;
            }
            cancellationTokenSource?.Cancel();
        }
    }
}
