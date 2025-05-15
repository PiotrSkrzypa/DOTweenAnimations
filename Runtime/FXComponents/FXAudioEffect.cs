using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public sealed class FXAudioEffect : BaseFXComponent
    {
        [SerializeField] AudioClip audioClip;
        [SerializeField] string uiAudioClipKey;
        [SerializeField] bool isUISound;

        protected override void PlayInternal()
        {
            base.PlayInternal();
            AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
        }
    }
}