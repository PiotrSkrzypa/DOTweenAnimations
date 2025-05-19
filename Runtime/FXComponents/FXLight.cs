using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXLight : BaseFXComponent
    {
        [SerializeField] Light light;

        protected override void StopInternal()
        {
            light.enabled = false;
        }
        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            await UniTask.Yield();
            light.enabled = true;
        }
    }
}
