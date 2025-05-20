using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class ColorTextMeshProTweenAnimation : BaseFXComponent
    {
        [SerializeField] TMP_Text textToColor;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            var handle = LMotion.Create(startColor, targetColor, Timing.Duration)
                .WithEase(Ease.OutQuad)
                .WithScheduler(Timing.GetScheduler())
                .BindToColor(textToColor);

            await handle.ToUniTask(cancellationToken);
        }

        protected override void StopInternal()
        {
            textToColor.color = startColor;
        }

        protected override void ResetInternal()
        {
            textToColor.color = startColor;
        }
    }
}
