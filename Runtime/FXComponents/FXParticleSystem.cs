using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXParticleSystem : BaseFXComponent
    {
        [SerializeField] ParticleSystem particleSystem;

        public override void Initialize()
        {
            particleSystem.Stop();
        }
        public override void Stop()
        {
            base.Stop();
            particleSystem.Stop();
        }
        override protected void PlayInternal()
        {
            particleSystem.time = 0;
            particleSystem.Play();
        }
    }
}
