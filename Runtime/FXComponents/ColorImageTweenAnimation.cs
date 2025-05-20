using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class ColorImageTweenAnimation : BaseFXComponent
    {
        [SerializeField] Image imageToColor;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            var handle = LMotion.Create(startColor, targetColor, Timing.Duration)
                .WithEase(Ease.OutQuad)
                .WithScheduler(Timing.GetScheduler())
                .BindToColor(imageToColor);

            await handle.ToUniTask(cancellationToken);
        }

        protected override void StopInternal()
        {
            imageToColor.color = startColor;
        }

        protected override void ResetInternal()
        {
            imageToColor.color = startColor;
        }
    }
}
