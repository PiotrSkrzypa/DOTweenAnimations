using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class EnableComponentTweenAnimation : BaseFXComponent
    {
        [SerializeField] Behaviour componentToEnable;
        [SerializeField] bool targetState = true;

        bool originalState;


        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            if (componentToEnable == null)
            {
                Debug.LogWarning("[EnableComponentTweenAnimation] componentToEnable is null.");
                return;
            }
            originalState = componentToEnable.enabled;
            componentToEnable.enabled = targetState;
            return;
        }
        protected override void StopInternal()
        {
            if (componentToEnable != null)
            {
                componentToEnable.enabled = originalState;
            }
        }
        protected override void ResetInternal()
        {
            if (componentToEnable != null)
            {
                componentToEnable.enabled = originalState;
            }
        }
    }
}
