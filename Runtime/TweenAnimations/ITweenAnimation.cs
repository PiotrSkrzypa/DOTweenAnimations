using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    public interface ITweenAnimation
    {
        bool IsRunning { get;}
        float Duration { get; set; }
        float Delay { get; set; }
        bool TimeScaleIndependent { get; }
        TweenAnimationCallback BeforeAnimationCallback { get; }
        TweenAnimationCallback AfterDelayCallback { get; }
        TweenAnimationCallback AfterAnimationCallback { get; }
        ITweenAnimation WithBeforeAnimationCallback(TweenAnimationCallback beforeAnimationCallback);
        ITweenAnimation WithAfterDelayCallback(TweenAnimationCallback afterDelayCallback);
        ITweenAnimation WithAfterAnimationCallback(TweenAnimationCallback afterAnimationCallback);
        void Play();
        void Stop();
    }
    public delegate void TweenAnimationCallback();
}