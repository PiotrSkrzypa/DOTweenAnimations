using System;
using PSkrzypa.UnityFX;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXTweenAnimation : BaseFXComponent
    {
        [SerializeField][SerializeReference] ITweenAnimation animation;

        public ITweenAnimation Animation { get => animation; }

        public override void Play()
        {
            animation.Play();
        }
        public override void Stop()
        {
            animation.Stop();
        }
    }
}