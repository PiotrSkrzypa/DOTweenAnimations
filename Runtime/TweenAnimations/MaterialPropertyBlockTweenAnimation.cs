using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Alchemy.Inspector;
using LitMotion;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class MaterialPropertyBlockTweenAnimation : ITweenAnimation
    {
        public bool IsRunning => isRunning;
        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool TimeScaleIndependent => timeScaleIndependent;

        bool isRunning;
        CancellationTokenSource cts;

        [SerializeField] private List<Renderer> meshRenderers;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float delay = 0f;
        [SerializeField] private bool timeScaleIndependent = true;
        [SerializeField][SerializeReference] private List<MaterialPropertyBlockParameter> parametersToAnimate;

        #region Callbacks

        public TweenAnimationCallback BeforeAnimationCallback => preparation;
        public TweenAnimationCallback AfterDelayCallback => afterDelayCallback;
        public TweenAnimationCallback AfterAnimationCallback => callbackAfterAnimation;

        TweenAnimationCallback preparation;
        TweenAnimationCallback afterDelayCallback;
        TweenAnimationCallback callbackAfterAnimation;

        public ITweenAnimation WithBeforeAnimationCallback(TweenAnimationCallback beforeAnimationCallback)
        {
            preparation = beforeAnimationCallback;
            return this;
        }

        public ITweenAnimation WithAfterDelayCallback(TweenAnimationCallback afterDelayCallback)
        {
            this.afterDelayCallback = afterDelayCallback;
            return this;
        }

        public ITweenAnimation WithAfterAnimationCallback(TweenAnimationCallback afterAnimationCallback)
        {
            callbackAfterAnimation = afterAnimationCallback;
            return this;
        }

        #endregion

        [Button]
        public void Play()
        {
            _ = PlayAsync();
        }

        private async UniTaskVoid PlayAsync()
        {
            Stop();

            if (meshRenderers == null || parametersToAnimate == null) return;

            isRunning = true;
            preparation?.Invoke();

            cts = new CancellationTokenSource();
            var token = cts.Token;

            if (delay > 0)
                await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: timeScaleIndependent, cancellationToken: token);

            afterDelayCallback?.Invoke();

            List<UniTask> allTasks = new();

            foreach (var renderer in meshRenderers)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                Renderer rendererTmp = renderer;
                renderer.GetPropertyBlock(block);

                var sequence = LSequence.Create();
                foreach (var param in parametersToAnimate)
                {
                    param.SetDefaultValue(rendererTmp, block);
                    sequence.Join(param.CreateMotion(block, duration, timeScaleIndependent));
                }
                sequence.Append(LMotion.Create(0f, 1f, duration)
                    .WithScheduler(timeScaleIndependent ? MotionScheduler.PostLateUpdateIgnoreTimeScale : MotionScheduler.PostLateUpdate)
                    .WithEase(Ease.OutQuad)
                    .Bind(block, (x, block) => rendererTmp.SetPropertyBlock(block)));
                allTasks.Add(sequence.Run().ToUniTask(token));
            }
            try
            {
                await UniTask.WhenAll(allTasks);
            }
            catch (OperationCanceledException) { }

            isRunning = false;
            callbackAfterAnimation?.Invoke();
        }
        [Button]
        public void Stop()
        {
            if (!isRunning) return;

            isRunning = false;
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            foreach (var renderer in meshRenderers)
            {
                MaterialPropertyBlock block = new();
                renderer.SetPropertyBlock(block);
            }
        }

        [Button]
        public void Reset()
        {
            isRunning = false;
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            foreach (var renderer in meshRenderers)
            {
                MaterialPropertyBlock block = new();
                renderer.SetPropertyBlock(block);
            }
        }

        public void InjectRenderersList(List<Renderer> renderers)
        {
            this.meshRenderers = renderers;
        }

        #region Parameter Definitions

        [Serializable]
        public abstract class MaterialPropertyBlockParameter
        {
            public string parameterName;
            public abstract MotionHandle CreateMotion(MaterialPropertyBlock block, float duration, bool timeScaleIndependent);
            public abstract void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock);
        }

        [Serializable]
        public class ColorParameter : MaterialPropertyBlockParameter
        {
            [ColorUsage(true, true)] public Color targetColor;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration, 
                bool timeScaleIndependent)
            {
                Color startColor = block.GetColor(parameterName);
                Color endColor = targetColor;

                var handle = LMotion.Create(startColor,endColor,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetColor(parameterName, x));
                return handle;
            }

            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetColor(parameterName, renderer.sharedMaterial.GetColor(parameterName));
            }
        }

        [Serializable]
        public class FloatParameter : MaterialPropertyBlockParameter
        {
            public float targetValue;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
                bool timeScaleIndependent)
            {
                float startValue = block.GetFloat(parameterName);

                var handle = LMotion.Create(startValue,targetValue,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetFloat(parameterName, x));
                return handle;
            }
            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetFloat(parameterName, renderer.sharedMaterial.GetFloat(parameterName));
            }
        }

        [Serializable]
        public class IntegerParameter : MaterialPropertyBlockParameter
        {
            public int targetValue;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
                bool timeScaleIndependent)
            {
                int startValue = block.GetInteger(parameterName);

                var handle = LMotion.Create(startValue,targetValue,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetInteger(parameterName, x));
                return handle;
            }
            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetInteger(parameterName, renderer.sharedMaterial.GetInteger(parameterName));
            }
        }

        [Serializable]
        public class Vector2Parameter : MaterialPropertyBlockParameter
        {
            public Vector2 targetValue;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
               bool timeScaleIndependent)
            {
                Vector2 startValue = block.GetVector(parameterName);

                var handle = LMotion.Create(startValue,targetValue,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetVector(parameterName, x));
                return handle;
            }
            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetVector(parameterName, renderer.sharedMaterial.GetVector(parameterName));
            }
        }

        [Serializable]
        public class Vector3Parameter : MaterialPropertyBlockParameter
        {
            public Vector3 targetValue;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
               bool timeScaleIndependent)
            {
                Vector3 startValue = block.GetVector(parameterName);

                var handle = LMotion.Create(startValue,targetValue,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetVector(parameterName, x));
                return handle;
            }
            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetVector(parameterName, renderer.sharedMaterial.GetVector(parameterName));
            }
        }

        [Serializable]
        public class Vector4Parameter : MaterialPropertyBlockParameter
        {
            public Vector4 targetValue;

            public override MotionHandle CreateMotion(MaterialPropertyBlock block, float duration,
               bool timeScaleIndependent)
            {
                Vector4 startValue = block.GetVector(parameterName);

                var handle = LMotion.Create(startValue,targetValue,duration).WithEase(Ease.OutQuad)
                .WithScheduler(timeScaleIndependent ? MotionScheduler.UpdateRealtime : MotionScheduler.Update)
                .Bind(block, (x, block) => block.SetVector(parameterName, x));
                return handle;
            }
            public override void SetDefaultValue(Renderer renderer, MaterialPropertyBlock materialPropertyBlock)
            {
                materialPropertyBlock.SetVector(parameterName, renderer.sharedMaterial.GetVector(parameterName));
            }
        }

        #endregion
    }
}
