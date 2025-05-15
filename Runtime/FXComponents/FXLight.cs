using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXLight : BaseFXComponent
    {
        [SerializeField] Light light;

        public override void Stop()
        {
            base.Stop();
            light.enabled = false;
        }
        protected override void PlayInternal()
        {
            light.enabled = true;
        }
    }
}
