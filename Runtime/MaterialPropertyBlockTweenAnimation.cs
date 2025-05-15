using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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
        [SerializeField] List<Renderer> meshRenderers;
        [SerializeField] float duration;
        [SerializeField] float delay;
        [SerializeField] bool timeScaleIndependent = true;
        [SerializeField] List<MaterialPropertyBlockParameter> parametersToAnimate;
        List<Sequence> sequences;

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
        abstract class MaterialPropertyBlockParameter
        {
            public string parameterName;
        }
        class ColorParameter : MaterialPropertyBlockParameter
        {
            [ColorUsage(true, true)]public Color color;
        }
        class FloatParameter : MaterialPropertyBlockParameter
        {
            public float value;
        }
        class IntegerParameter : MaterialPropertyBlockParameter
        {
            public int value;
        }
        class Vector2Parameter : MaterialPropertyBlockParameter
        {
            public Vector2 value;
        }
        class Vector3Parameter : MaterialPropertyBlockParameter
        {
            public Vector3 value;
        }
        class Vector4Parameter : MaterialPropertyBlockParameter
        {
            public Vector4 value;
        }
        public void Play()
        {
            isRunning = true;
            if (preparation != null)
            {
                preparation();
            }
            if (sequences == null)
            {
                sequences = new List<Sequence>();
            }
            else
            {
                for (int i = 0; i < sequences.Count; i++)
                {
                    sequences[i].Kill();
                }
            }

            for (int i = 0; i < meshRenderers.Count; i++)
            {
                Renderer meshRenderer = meshRenderers[i];
                MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(materialPropertyBlock);
                for (int j = 0; j < parametersToAnimate.Count; j++)
                {
                    AnimatePropertyBlockParameter(materialPropertyBlock, parametersToAnimate[j], meshRenderer);
                }
            }
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delay);
            if (afterDelayCallback != null)
            {
                sequence.AppendCallback(() => afterDelayCallback());
            }
            sequence.AppendInterval(duration);
            sequence.onComplete += () =>
            {
                isRunning = false;
                InformAboutAnimationEnd(callbackAfterAnimation);
            };
            sequences.Add(sequence);
            sequence.Play();

        }
        private void AnimatePropertyBlockParameter(MaterialPropertyBlock materialPropertyBlock, MaterialPropertyBlockParameter parameterToAnimate, Renderer meshRenderer)
        {
            string parameterName = parameterToAnimate.parameterName;
            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(timeScaleIndependent);
            sequence.AppendInterval(delay);
            if (meshRenderer.sharedMaterial.HasProperty(parameterName))
            {
                if (parameterToAnimate is ColorParameter)
                {
                    if (materialPropertyBlock.GetColor(parameterName) == default(Color))
                    {
                        materialPropertyBlock.SetColor(parameterName, meshRenderer.sharedMaterial.GetColor(parameterName));
                    }
                    Color color = ( (ColorParameter)parameterToAnimate ).color;
                    sequence.Append(DOTween.To(() => materialPropertyBlock.GetColor(parameterName), value => materialPropertyBlock.SetColor(parameterName, value), color, duration));
                    sequence.onComplete += () => { materialPropertyBlock.SetColor(parameterName, color); meshRenderer.SetPropertyBlock(materialPropertyBlock); };
                }
                else if (parameterToAnimate is FloatParameter)
                {
                    float targetValue = ( (FloatParameter)parameterToAnimate ).value;
                    sequence.Append(DOTween.To(() => materialPropertyBlock.GetFloat(parameterName), value => materialPropertyBlock.SetFloat(parameterName, value), targetValue, duration));
                    sequence.onComplete += () => { materialPropertyBlock.SetFloat(parameterName, targetValue); meshRenderer.SetPropertyBlock(materialPropertyBlock); };
                }
                else if (parameterToAnimate is IntegerParameter)
                {
                    int targetValue = ( (IntegerParameter)parameterToAnimate ).value;
                    sequence.Append(DOTween.To(() => materialPropertyBlock.GetInt(parameterName), value => materialPropertyBlock.SetInt(parameterName, value), targetValue, duration));
                    sequence.onComplete += () => { materialPropertyBlock.SetInt(parameterName, targetValue); meshRenderer.SetPropertyBlock(materialPropertyBlock); };
                }
                else if (parameterToAnimate is Vector2Parameter)
                {
                    Vector4 targetValue = ( (Vector2Parameter)parameterToAnimate ).value;
                    sequence.Append(DOTween.To(() => materialPropertyBlock.GetVector(parameterName), value => materialPropertyBlock.SetVector(parameterName, targetValue), targetValue, duration));
                    sequence.onComplete += () => { materialPropertyBlock.SetVector(parameterName, targetValue); meshRenderer.SetPropertyBlock(materialPropertyBlock); };
                }
                else if (parameterToAnimate is Vector3Parameter)
                {
                    Vector4 targetValue = ( (Vector3Parameter)parameterToAnimate ).value;
                    sequence.Append(DOTween.To(() => materialPropertyBlock.GetVector(parameterName), value => materialPropertyBlock.SetVector(parameterName, targetValue), targetValue, duration));
                    sequence.onComplete += () => { materialPropertyBlock.SetVector(parameterName, targetValue); meshRenderer.SetPropertyBlock(materialPropertyBlock); };
                }
                else if (parameterToAnimate is Vector4Parameter)
                {
                    Vector4 targetValue = ( (Vector4Parameter)parameterToAnimate ).value;
                    sequence.Append(DOTween.To(() => materialPropertyBlock.GetVector(parameterName), value => materialPropertyBlock.SetVector(parameterName, targetValue), targetValue, duration));
                    sequence.onComplete += () => { materialPropertyBlock.SetVector(parameterName, targetValue); meshRenderer.SetPropertyBlock(materialPropertyBlock); };
                }
                sequence.OnUpdate(() => meshRenderer.SetPropertyBlock(materialPropertyBlock));
            }
            else
            {
                sequence.AppendInterval(duration);
            }
            sequence.SetLink(meshRenderer.gameObject, LinkBehaviour.KillOnDestroy);
            sequence.Play();
            sequences.Add(sequence);
        }
        private void InformAboutAnimationEnd(TweenAnimationCallback callbackAfterAnimation)
        {
            if (callbackAfterAnimation != null)
            {
                callbackAfterAnimation();
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                if (sequences != null)
                {
                    for (int i = 0; i < sequences.Count; i++)
                    {
                        sequences[i].Kill();
                    }
                }
            }
        }
        public void InjectRenderersList(List<Renderer> renderers)
        {
            this.meshRenderers = renderers;
        }
    }
}