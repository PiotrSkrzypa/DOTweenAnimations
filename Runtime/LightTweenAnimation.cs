using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PSkrzypa.DOTweenAnimations
{
    [Serializable]
    public class LightTweenAnimation : ITweenAnimation
    {
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;
        public List<ITweenAnimation> FollowingAnimations { get => followingAnimations; set => followingAnimations = value; }
        public List<ITweenAnimation> AdditionalAnimations { get => additionalAnimations; set => additionalAnimations = value; }
        public bool IsRunning { get => isRunning; }

        bool isRunning;
        [SerializeField] Light light;
        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] float startIntensity;
        [SerializeField] float targetIntensity;
        [SerializeField][SerializeReference] List<ITweenAnimation> additionalAnimations;
        [SerializeField][SerializeReference] List<ITweenAnimation> followingAnimations;
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
            PlayAdditionalAnimations();
            float animationDelay = delay;
            if (light.intensity != startIntensity)
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
            sequence.Append(light.DOIntensity(targetIntensity, duration).OnComplete(() =>
            {
                isRunning = false;
                InformAboutAnimationEnd(callbackAfterAnimation);
                PlayFollowingAnimations();
            }));
            sequence.SetLink(light.gameObject, LinkBehaviour.KillOnDestroy);
            sequence.Play();
        }
        private void InformAboutAnimationEnd(TweenAnimationCallback callbackAfterAnimation)
        {
            if (callbackAfterAnimation != null)
            {
                callbackAfterAnimation();
            }
        }

        private void PlayFollowingAnimations()
        {
            if (followingAnimations != null)
            {
                for (int j = 0; j < followingAnimations.Count; j++)
                {
                    followingAnimations[j].Play();
                }
            }
        }

        private void PlayAdditionalAnimations()
        {
            if (additionalAnimations != null)
            {
                for (int i = 0; i < additionalAnimations.Count; i++)
                {
                    additionalAnimations[i].Play();
                }
            }
        }

        public void StopAllTweens()
        {
            if (isRunning)
            {
                isRunning = false;
                light.intensity = startIntensity;
                if (sequence != null)
                {
                    sequence.Kill();
                }
            }
            if (additionalAnimations != null)
            {
                for (int i = 0; i < additionalAnimations.Count; i++)
                {
                    additionalAnimations[i].StopAllTweens();
                }
            }
            if (followingAnimations != null)
            {
                for (int i = 0; i < followingAnimations.Count; i++)
                {
                    followingAnimations[i].StopAllTweens();
                }
            }
        }
    }
}
