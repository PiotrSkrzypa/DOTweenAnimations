using System;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXFXObject : BaseFXComponent
    {
        [SerializeField] FXPlayer fxObject;

        protected override void PlayInternal()
        {
            //VFXDirector.Instance.PlayVFXOnPosition(_vfxObject, coroutineRunner.transform.position, coroutineRunner.transform.rotation);
            fxObject.Play();
        }
    }
}