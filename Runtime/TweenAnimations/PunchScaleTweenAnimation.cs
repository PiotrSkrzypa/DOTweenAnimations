using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PunchScaleTweenAnimation : ITweenAnimation
    {
        public bool IsRunning { get => isRunning; }
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;
        public float Elasticity { get => elasticity; set => elasticity = value; }
        public int Vibrato { get => vibrato; set => vibrato = value; }

        bool isRunning;
        [SerializeField] List<Transform> transformsToScale;
        [SerializeField] float duration;
        [SerializeField] float elasticity;
        [SerializeField] int vibrato;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] Vector3 startingScale;
        [SerializeField] Vector3 punchStrength;
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
            for (int i = 0; i < transformsToScale.Count; i++)
            {
                transformsToScale[i].localScale = startingScale;
            }

            for (int i = 0; i < transformsToScale.Count; i++)
            {
                Transform transformToScale = transformsToScale[i];
                int index = i;
                Sequence sequence = DOTween.Sequence();
                sequence.SetUpdate(timeScaleIndependent);
                sequence.AppendInterval(delay);
                if (afterDelayCallback != null)
                {
                    sequence.AppendCallback(() => afterDelayCallback());
                }
                sequence.Append(transformToScale.DOPunchScale(punchStrength, duration));
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
                sequence.SetLink(transformToScale.gameObject, LinkBehaviour.KillOnDestroy);
                sequence.Play();
                sequences.Add(sequence);
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                for (int i = 0; i < transformsToScale.Count; i++)
                {
                    transformsToScale[i].localScale = startingScale;
                }
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