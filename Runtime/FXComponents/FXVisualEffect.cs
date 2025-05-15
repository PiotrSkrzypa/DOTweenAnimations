using System;
using UnityEngine;
using UnityEngine.VFX;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXVisualEffect : BaseFXComponent
    {
        [SerializeField] VisualEffect visualEffect;

        public override void Initialize()
        {
            base.Initialize();
            visualEffect.Stop();
        }

        public override void Stop()
        {
            base.Stop();
            visualEffect.Stop();
        }
        protected override void PlayInternal()
        {
            visualEffect.Reinit();
            visualEffect.Play();
        }
    }
}