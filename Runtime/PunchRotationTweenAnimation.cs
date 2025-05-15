using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PSkrzypa.DOTweenAnimations
{
    [Serializable]
    public class PunchRotationTweenAnimation : ITweenAnimation
    {
        public bool IsRunning => isRunning;
        public float Duration { get => duration; set => duration = value; }
        public float Elasticity { get => elasticity; set => elasticity = value; }
        public int Vibrato { get => vibrato; set => vibrato = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;


        bool isRunning;
        [SerializeField] List<Transform> transformsToRotate;
        [SerializeField] float duration;
        [SerializeField] float elasticity;
        [SerializeField] int vibrato;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] Vector3 punch;
        List<Sequence> sequences;

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
            if (sequences == null)
            {
                sequences = new List<Sequence>();
            }
            else
            {
                for (int i = 0; i < sequences.Count; i++)
                {
                    sequences[i].Kill();
                }
            }

            for (int i = 0; i < transformsToRotate.Count; i++)
            {
                Transform transformToPosition = transformsToRotate[i];
                int index = i;
                Sequence sequence = DOTween.Sequence();
                sequence.SetUpdate(timeScaleIndependent);
                sequence.AppendInterval(delay);
                if (afterDelayCallback != null)
                {
                    sequence.AppendCallback(() => afterDelayCallback());
                }
                sequence.Append(transformToPosition.DOPunchRotation(punch, duration, vibrato, elasticity));
                sequence.AppendCallback(() =>
               {
                   if (index == 0)
                   {
                       isRunning = false;
                       if (callbackAfterAnimation != null)
                       {
                           callbackAfterAnimation();
                       }
                   }
               });
                sequence.SetLink(transformToPosition.gameObject, LinkBehaviour.KillOnDestroy);
                sequence.Play();
                sequences.Add(sequence);
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                if (sequences != null)
                {
                    for (int i = 0; i < sequences.Count; i++)
                    {
                        sequences[i].Kill();
                    }
                } 
            }
        }
    }
}