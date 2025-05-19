using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public sealed class FXAudioEffect : BaseFXComponent
    {
        [SerializeField] AudioClip audioClip;
        [SerializeField] string uiAudioClipKey;
        [SerializeField] bool isUISound;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            await UniTask.Delay((int)( Timing.Duration * 1000 ), cancellationToken: cancellationToken);
            AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
        }
    }
}