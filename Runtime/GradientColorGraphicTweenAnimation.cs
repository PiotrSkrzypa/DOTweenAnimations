using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.DOTweenAnimations
{
    [Serializable]
    public class GradientColorGraphicTweenAnimation : ITweenAnimation
    {
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;
        public bool IsRunning { get => isRunning; }

        bool isRunning;
        [SerializeField] Graphic graphicToColor;
        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] float gradientSamplingResolution = 30f;
        [SerializeField] Gradient gradient;
        Color[] colorSamples;
        Color originalColor;
        Sequence sequence;

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
        public void Play()
        {
            isRunning = true;
            colorSamples = new Color[(int)gradientSamplingResolution];
            for (int i = 0; i < gradientSamplingResolution; i++)
            {
                Color colorSample = gradient.Evaluate(i/(gradientSamplingResolution-1));
                colorSamples[i] = colorSample;
            }
            if (preparation != null)
            {
                preparation();
            }
            if (sequence != null)
            {
                sequence.Kill();
            }
            float animationDelay = delay;
            originalColor = graphicToColor.color;
            sequence = DOTween.Sequence();
            sequence.SetUpdate(timeScaleIndependent);
            sequence.AppendInterval(animationDelay);
            if (afterDelayCallback != null)
            {
                sequence.AppendCallback(() => afterDelayCallback());
            }
            for (int i = 0; i < colorSamples.Length; i++)
            {
                Tween tween = graphicToColor.DOColor(colorSamples[i], duration / colorSamples.Length);
                if (i == colorSamples.Length - 1)
                {
                    tween.OnComplete(() =>
                    {
                        isRunning = false;
                        InformAboutAnimationEnd(callbackAfterAnimation);
                    });
                }
                sequence.Append(tween);
            }
            sequence.SetLink(graphicToColor.gameObject, LinkBehaviour.KillOnDestroy);
            sequence.Play();
        }

        private void InformAboutAnimationEnd(TweenAnimationCallback callbackAfterAnimation)
        {
            if (callbackAfterAnimation != null)
            {
                callbackAfterAnimation();
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                graphicToColor.color = originalColor;
                if (sequence != null)
                {
                    sequence.Kill();
                }
            }
        }
    }
}