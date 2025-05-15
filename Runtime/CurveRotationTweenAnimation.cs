using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PSkrzypa.DOTweenAnimations
{
    [Serializable]
    public class CurveRotationTweenAnimation : ITweenAnimation
    {
        public bool IsRunning => isRunning;
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;


        bool isRunning;
        [SerializeField] List<Transform> transformsToRotate;
        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] Vector3 startingRotation;
        [SerializeField] float samplingResolution = 30f;
        [SerializeField] float curvesMultiplier = 1f;
        [SerializeField] AnimationCurve xRotationCurve;
        [SerializeField] AnimationCurve yRotationCurve;
        [SerializeField] AnimationCurve zRotationCurve;
        [SerializeField] PathType pathType;
        [SerializeField] Ease easeType = Ease.Linear;
        Vector3[] rotationWaypoints;
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
            for (int i = 0; i < transformsToRotate.Count; i++)
            {
                transformsToRotate[i].eulerAngles = startingRotation;
            }

            for (int i = 0; i < transformsToRotate.Count; i++)
            {
                Transform transformToRotate = transformsToRotate[i];
                int index = i;
                Sequence sequence = DOTween.Sequence();
                sequence.SetUpdate(timeScaleIndependent);
                sequence.AppendInterval(delay);
                if (afterDelayCallback != null && i == 0)
                {
                    sequence.AppendCallback(() => afterDelayCallback());
                }
                for (int j = 0; j < rotationWaypoints.Length; j++)
                {
                    if (j == rotationWaypoints.Length - 1 && index == 0)
                    {
                        sequence.Append(transformToRotate.DORotate(rotationWaypoints[j], duration / rotationWaypoints.Length).SetEase(easeType).OnComplete(() =>
                        {
                            isRunning = false;
                            InformAboutAnimationEnd(callbackAfterAnimation);
                        }));
                    }
                    else
                    {
                        sequence.Append(transformToRotate.DORotate(rotationWaypoints[j], duration / rotationWaypoints.Length));
                    }
                    //sequence.Append(tween);
                }
                sequence.SetLink(transformToRotate.gameObject, LinkBehaviour.KillOnDestroy);
                sequence.Play();
                sequences.Add(sequence);
            }
        }

        private void SampleCurves()
        {
            rotationWaypoints = new Vector3[(int)samplingResolution];
            for (int i = 0; i < samplingResolution; i++)
            {
                Vector3 eulerRotation = new Vector3();
                if (xRotationCurve != null && xRotationCurve.keys?.Length > 0)
                {
                    eulerRotation.x = xRotationCurve.Evaluate(i / ( samplingResolution - 1 ));
                }
                if (yRotationCurve != null && yRotationCurve.keys?.Length > 0)
                {
                    eulerRotation.y = yRotationCurve.Evaluate(i / ( samplingResolution - 1 ));
                }
                if (zRotationCurve != null && zRotationCurve.keys?.Length > 0)
                {
                    eulerRotation.z = zRotationCurve.Evaluate(i / ( samplingResolution - 1 ));
                }
                eulerRotation *= curvesMultiplier;
                rotationWaypoints[i] = eulerRotation;
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
                for (int i = 0; i < transformsToRotate.Count; i++)
                {
                    transformsToRotate[i].localPosition = startingRotation;
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