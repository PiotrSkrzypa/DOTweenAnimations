using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSkrzypa.DOTweenAnimations
{
    [Serializable]
    public class EmptyTweenAnimation : ITweenAnimation
    {
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => true;
        public List<ITweenAnimation> FollowingAnimations { get => followingAnimations; set => followingAnimations = value; }
        public List<ITweenAnimation> AdditionalAnimations { get => additionalAnimations; set => additionalAnimations = value; }
        public bool IsRunning { get => isRunning; }

        bool isRunning;
        float duration = 0;
        float delay = 0;
        List<ITweenAnimation> additionalAnimations;
        List<ITweenAnimation> followingAnimations;


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
            PlayAdditionalAnimations();
            isRunning = false;
            InformAboutAnimationEnd(callbackAfterAnimation);
            PlayFollowingAnimations();
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
        }
    }
}