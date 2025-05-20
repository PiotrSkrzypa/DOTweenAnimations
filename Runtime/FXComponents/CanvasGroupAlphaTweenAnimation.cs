using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class CanvasGroupAlphaTweenAnimation : BaseFXComponent
    {
        [SerializeField] CanvasGroup targetCanvasGroup;
        [SerializeField] float startAlpha;
        [SerializeField] float targetAlpha;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            var handle = LMotion.Create(targetCanvasGroup.alpha, targetAlpha, Timing.Duration)
                .WithEase(LitMotion.Ease.OutQuad).WithScheduler(Timing.GetScheduler())
                .BindToAlpha(targetCanvasGroup);
            await handle.ToUniTask(cancellationToken);
        }
        protected override void StopInternal()
        {
            targetCanvasGroup.alpha = startAlpha;
        }
        protected override void ResetInternal()
        {
            targetCanvasGroup.alpha = startAlpha;
        }
    }
}
