using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;
using System.Threading;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class PunchPositionTweenAnimation : BaseFXComponent
    {
        public int Frequency { get => frequency; set => frequency = value; }
        public float Damping { get => damping; set => damping = value; }

        [SerializeField] private Transform transformToMove;
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private float damping = 0.5f;
        [SerializeField] private int frequency = 10;
        [SerializeField] private Vector3 punch;

        Vector3 originalPosition;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            var scheduler = Timing.GetScheduler();

            originalPosition = useLocalSpace ? transformToMove.localPosition : transformToMove.position;

            var motionBuilder = LMotion.Punch.Create(originalPosition, punch, Timing.Duration)
                .WithFrequency(frequency)
                .WithDampingRatio(damping)
                .WithScheduler(scheduler);
            UniTask uniTask = useLocalSpace ? 
                motionBuilder.Bind(transformToMove, (v, tr) => transformToMove.localPosition = v)
                .ToUniTask(cancellationToken) :
                motionBuilder.Bind(transformToMove, (v, tr) => transformToMove.position = v)
                .ToUniTask(cancellationToken);

            await uniTask;
        }
        protected override void StopInternal()
        {
            if (useLocalSpace)
            {
                transformToMove.localPosition = originalPosition;
            }
            else
            {
                transformToMove.position = originalPosition;
            }
        }
        protected override void ResetInternal()
        {
            if (useLocalSpace)
            {
                transformToMove.localPosition = originalPosition;
            }
            else
            {
                transformToMove.position = originalPosition;
            }
        }
    }
}
