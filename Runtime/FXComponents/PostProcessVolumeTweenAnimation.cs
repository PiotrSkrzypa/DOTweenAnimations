using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PostProcessVolumeTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Volume volume;
        [SerializeField] private float startWeight = 0f;
        [SerializeField] private float targetWeight = 1f;
        [SerializeField] private Ease ease = Ease.OutQuad;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            if (volume == null)
            {
                Debug.LogWarning("[PostProcessVolumeTweenAnimation] Volume is null.");
                return;
            }

            volume.weight = startWeight;

            var scheduler = Timing.GetScheduler();

            var tween = LMotion.Create(startWeight, targetWeight, Timing.Duration)
                .WithEase(ease)
                .WithScheduler(scheduler)
                .Bind(volume, (x, v) => v.weight = x);

            await tween.ToUniTask(cancellationToken);
        }

        protected override void StopInternal()
        {
            if (volume != null)
                volume.weight = startWeight;
        }

        protected override void ResetInternal()
        {
            if (volume != null)
                volume.weight = startWeight;
        }
    }
}
