using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXTrailRenderer : BaseFXComponent
    {
        [SerializeField] TrailRenderer trailRenderer;

        protected override void StopInternal()
        {
            trailRenderer.emitting = false;
        }
        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            await UniTask.Yield();
            trailRenderer.emitting = true;
        }
    }
}
