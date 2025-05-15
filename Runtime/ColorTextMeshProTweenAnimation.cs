using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.DOTweenAnimations
{
    [Serializable]
    public class ColorTextMeshProTweenAnimation : ITweenAnimation
    {
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool IsRunning { get => isRunning; }
        public bool TimeScaleIndependent => timeScaleIndependent;

        bool isRunning;
        [SerializeField] TMP_Text textToColor;
        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;
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
            if (preparation != null)
            {
                preparation();
            }
            if (sequence != null)
            {
                sequence.Kill();
            }
            float animationDelay = delay;
            if (textToColor.color != startColor)
            {
                animationDelay = 0f;
            }
            sequence = DOTween.Sequence();
            sequence.SetUpdate(timeScaleIndependent);
            sequence.AppendInterval(animationDelay);
            if (afterDelayCallback != null)
            {
                sequence.AppendCallback(() => afterDelayCallback());
            }
            sequence.Append(textToColor.DOColor(targetColor, duration).OnComplete(() =>
            {
                isRunning = false;
                InformAboutAnimationEnd(callbackAfterAnimation);
            }));
            sequence.SetLink(textToColor.gameObject, LinkBehaviour.KillOnDestroy);
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
                textToColor.color = startColor;
                if (sequence != null)
                {
                    sequence.Kill();
                }
            }
        }
    }
}