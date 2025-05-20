using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PositionTweenAnimation : BaseFXComponent
    {
        [SerializeField] private Transform transformToMove;
        [SerializeField] bool useLocalSpace = true;
        [SerializeField] private Vector3 startingPosition;
        [SerializeField] private Vector3 targetPosition;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            if (transformToMove == null)
                return;

            ResetPosition();

            var scheduler = Timing.GetScheduler();

            var morionBuilder = LMotion.Create(startingPosition, targetPosition, Timing.Duration)
                    .WithEase(Ease.OutQuad)
                    .WithScheduler(scheduler);
            var handle = useLocalSpace ? morionBuilder.Bind(transformToMove, (x, t) => t.localPosition = x) :
                    morionBuilder.Bind(transformToMove, (x, t) => t.position = x);

            await handle.ToUniTask(cancellationToken);
        }
        private void ResetPosition()
        {
            if (useLocalSpace)
            {
                transformToMove.localPosition = startingPosition;
            }
            else
            {
                transformToMove.position = startingPosition;
            }
        }
        protected override void StopInternal()
        {
            ResetPosition();
        }
        protected override void ResetInternal()
        {
            ResetPosition();
        }

    }
}
