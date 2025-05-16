using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Alchemy.Inspector;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class LightTweenAnimation : ITweenAnimation
    {
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;
        public bool IsRunning => isRunning;

        bool isRunning;
        CancellationTokenSource cts;

        [SerializeField] Light light;
        [SerializeField] float duration = 1f;
        [SerializeField] float delay = 0f;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] float startIntensity = 0f;
        [SerializeField] float targetIntensity = 1f;

        float originalIntensity;

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
            if (light == null)
            {
                Debug.LogWarning("[LightTweenAnimation] Light reference is null.");
                return;
            }

            Stop();

            isRunning = true;
            preparation?.Invoke();

            originalIntensity = light.intensity;
            light.intensity = startIntensity;

            cts = new CancellationTokenSource();
            var token = cts.Token;

            if (delay > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: timeScaleIndependent, cancellationToken: token);

            afterDelayCallback?.Invoke();

            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (token.IsCancellationRequested) return;

                float t = elapsed / duration;
                light.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                await UniTask.Yield(PlayerLoopTiming.Update);

                elapsed += timeScaleIndependent ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            light.intensity = targetIntensity;
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

            if (light != null)
                light.intensity = startIntensity;
        }

        [Button]
        public void Reset()
        {
            Stop();
            if (light != null)
                light.intensity = startIntensity;
        }
    }
}
