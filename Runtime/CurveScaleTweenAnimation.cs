using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PSkrzypa.DOTweenAnimations
{
    [Serializable]
    public class CurveScaleTweenAnimation : ITweenAnimation
    {
        public bool IsRunning => isRunning;
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;


        bool isRunning;
        [SerializeField] List<Transform> transformsToScale;
        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] Vector3 startingScale;
        [SerializeField] float samplingResolution = 30f;
        [SerializeField] AnimationCurve xScaleCurve;
        [SerializeField] AnimationCurve yScaleCurve;
        [SerializeField] AnimationCurve zScaleCurve;
        [SerializeField] PathType pathType;
        Vector3[] scaleWaypoints;
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
            SampleCurves();
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
                Transform transformToRotate = transformsToScale[i];
                int index = i;
                Sequence sequence = DOTween.Sequence();
                sequence.SetUpdate(timeScaleIndependent);
                sequence.AppendInterval(delay);
                if (afterDelayCallback != null && i == 0)
                {
                    sequence.AppendCallback(() => afterDelayCallback());
                }
                for (int j = 0; j < scaleWaypoints.Length; j++)
                {
                    Tween tween = transformToRotate.DOScale(scaleWaypoints[j], duration / scaleWaypoints.Length);
                    if (j == scaleWaypoints.Length - 1 && index == 0)
                    {
                        tween.OnComplete(() =>
                        {
                            isRunning = false;
                            InformAboutAnimationEnd(callbackAfterAnimation);
                        });
                    }
                    sequence.Append(tween);
                }
                sequence.SetLink(transformToRotate.gameObject, LinkBehaviour.KillOnDestroy);
                sequence.Play();
                sequences.Add(sequence);
            }
        }

        private void SampleCurves()
        {
            scaleWaypoints = new Vector3[(int)samplingResolution];
            for (int i = 0; i < samplingResolution; i++)
            {
                Vector3 scale = new Vector3();
                if (xScaleCurve != null && xScaleCurve.keys?.Length > 0)
                {
                    scale.x = xScaleCurve.Evaluate(i / ( samplingResolution - 1 ));
                }
                if (yScaleCurve != null && yScaleCurve.keys?.Length > 0)
                {
                    scale.y = yScaleCurve.Evaluate(i / ( samplingResolution - 1 ));
                }
                if (zScaleCurve != null && zScaleCurve.keys?.Length > 0)
                {
                    scale.z = zScaleCurve.Evaluate(i / ( samplingResolution - 1 ));
                }
                scaleWaypoints[i] = scale;
            }
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
                for (int i = 0; i < transformsToScale.Count; i++)
                {
                    transformsToScale[i].localPosition = startingScale;
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