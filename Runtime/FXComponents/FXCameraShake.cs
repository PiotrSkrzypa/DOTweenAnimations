using UnityEngine;
using DG.Tweening;
using System;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXCameraShake : BaseFXComponent
    {
        [SerializeField] float shakeDuration;
        [SerializeField] int vibrato = 10;
        [SerializeField] float randomness = 90f;
        [SerializeField] bool fadeOut;
        [SerializeField] Ease easeType = Ease.OutQuad;
        [SerializeField] Vector3 movementShakeMagnitude;
        [SerializeField] float zRotationShakeMagnitude;
        Tweener cameraMovementTweener;
        Tweener cameraRotationTweener;

        public override void Stop()
        {
            base.Stop();
            cameraMovementTweener.Kill();
            cameraRotationTweener.Kill();
        }
        protected override void PlayInternal()
        {
            Shake();
        }
        private void Shake()
        {
            Camera camera = Camera.main;
            if (movementShakeMagnitude != Vector3.zero)
            {
                cameraMovementTweener = camera.DOShakePosition(shakeDuration, movementShakeMagnitude, vibrato, randomness, fadeOut);
            }
            if (zRotationShakeMagnitude != 0)
            {
                cameraRotationTweener = camera.DOShakeRotation(shakeDuration, new Vector3(0, 0, zRotationShakeMagnitude), vibrato, randomness, fadeOut).SetEase(easeType);
            }
        }

    }
}