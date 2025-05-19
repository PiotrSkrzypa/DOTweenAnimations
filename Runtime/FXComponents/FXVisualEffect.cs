using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        protected override void StopInternal()
        {
            visualEffect.Stop();
        }
        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            visualEffect.Reinit();
            visualEffect.Play();
        }
    }
}