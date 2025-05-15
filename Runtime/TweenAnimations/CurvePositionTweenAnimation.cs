using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CurvePositionTweenAnimation : ITweenAnimation
    {
        public bool IsRunning => isRunning;
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;


        bool isRunning;
        [SerializeField] List<Transform> transformsToMove;
        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] bool useUIScale;
        [SerializeField] Vector3 startingPosition;
        [SerializeField] float samplingResolution = 30f;
        [SerializeField] AnimationCurve xPositionCurve;
        [SerializeField] AnimationCurve yPositionCurve;
        [SerializeField] AnimationCurve zPositionCurve;
        [SerializeField] PathType pathType;
        [SerializeField] Ease easeType = Ease.Linear;
        Vector3[] waypoints;
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
            for (int i = 0; i < transformsToMove.Count; i++)
            {
                transformsToMove[i].localPosition = startingPosition;
            }

            for (int i = 0; i < transformsToMove.Count; i++)
            {
                Transform transformToPosition = transformsToMove[i];
                int index = i;
                Sequence sequence = DOTween.Sequence();
                sequence.SetUpdate(timeScaleIndependent);
                sequence.AppendInterval(delay);
                if (afterDelayCallback != null)
                {
                    sequence.AppendCallback(() => afterDelayCallback());
                }
                sequence.Append(transformToPosition.DOLocalPath(waypoints, duration, pathType).SetEase(easeType));
                sequence.AppendCallback(() =>
                {
                    if (index == 0)
                    {
                        isRunning = false;
                        InformAboutAnimationEnd(callbackAfterAnimation);
                    }
                });
                sequence.SetLink(transformToPosition.gameObject, LinkBehaviour.KillOnDestroy);
                sequence.Play();
                sequences.Add(sequence);
            }
        }


        private static void InformAboutAnimationEnd(TweenAnimationCallback callbackAfterAnimation)
        {
            if (callbackAfterAnimation != null)
            {
                callbackAfterAnimation();
            }
        }

        private void SampleCurves()
        {
            waypoints = new Vector3[(int)samplingResolution];
            for (int i = 0; i < samplingResolution; i++)
            {
                Vector3 waypoint = new Vector3();
                if (xPositionCurve != null && xPositionCurve.keys?.Length > 0)
                {
                    waypoint.x = xPositionCurve.Evaluate(i / ( samplingResolution - 1 ));
                }
                if (yPositionCurve != null && yPositionCurve.keys?.Length > 0)
                {
                    waypoint.y = yPositionCurve.Evaluate(i / ( samplingResolution - 1 ));
                }
                if (zPositionCurve != null && zPositionCurve.keys?.Length > 0)
                {
                    waypoint.z = zPositionCurve.Evaluate(i / ( samplingResolution - 1 ));
                }
                waypoints[i] = waypoint;
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                for (int i = 0; i < transformsToMove.Count; i++)
                {
                    transformsToMove[i].localPosition = startingPosition;
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