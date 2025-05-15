using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXTrailRenderer : BaseFXComponent
    {
        [SerializeField] TrailRenderer trailRenderer;


        public override void Stop()
        {
            base.Stop();
            trailRenderer.emitting = false;
        }
        protected override void PlayInternal()
        {
            trailRenderer.emitting = true;
        }
    }
}
