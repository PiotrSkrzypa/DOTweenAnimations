using UnityEngine;
using System;
using LitMotion;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class FXCameraShake : BaseFXComponent
    {
        [SerializeField] float shakeDuration;
        [SerializeField] int frequency = 10;
        [SerializeField] float damping = 0.5f;
        [SerializeField] bool fadeOut;
        [SerializeField] Ease easeType = Ease.OutQuad;
        [SerializeField] Vector3 movementShakeMagnitude;
        [SerializeField] float zRotationShakeMagnitude;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            await UniTask.Delay((int)( Timing.Duration * 1000 ), cancellationToken: cancellationToken);
            Camera camera = Camera.main;
            Vector3 originalPosiiton = camera.transform.localPosition;
            Vector3 originalRotation = camera.transform.localEulerAngles;
            List<UniTask> tasks = new List<UniTask>();
            var scheduler = Timing.GetScheduler();
            if (movementShakeMagnitude != Vector3.zero)
            {
                UniTask uniTask = LMotion.Shake.Create(originalPosiiton, movementShakeMagnitude, Timing.Duration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithScheduler(scheduler)
                .Bind(camera.transform, (v, tr) => tr.localPosition = v)
                .ToUniTask(cancellationToken);
                tasks.Add(uniTask);
            }
            if (zRotationShakeMagnitude != 0)
            {
                Vector3 rotationMagnitude = new Vector3(0, 0, zRotationShakeMagnitude);
                UniTask uniTask = LMotion.Shake.Create(originalRotation, rotationMagnitude, Timing.Duration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithScheduler(scheduler)
                .Bind(camera.transform, (v, tr) => tr.localEulerAngles = v)
                .ToUniTask(cancellationToken);
                tasks.Add(uniTask);
            }
            await UniTask.WhenAll(tasks);
        }

    }
}