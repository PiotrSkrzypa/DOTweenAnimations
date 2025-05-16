using UnityEngine;
using Alchemy.Inspector;

namespace PSkrzypa.UnityFX
{
    public class FXPlayer : MonoBehaviour
    {
        [SerializeField] bool playOnAwake;
        [SerializeField] bool playOnEnable;
        [SerializeField] bool stopOnDisable;
        [SerializeField][SerializeReference] BaseFXComponent[] components;
        public BaseFXComponent[] Components { get => components; }

        protected bool initialized;

        protected virtual void Awake()
        {
            if (!initialized)
            {
                Initialize();
            }
            if (playOnAwake)
            {
                Play();
            }
        }
        public void Initialize()
        {
            initialized = true;
        }
        private void OnEnable()
        {
            if (!initialized)
            {
                Initialize();
            }
            if (playOnEnable)
            {
                Play();
            }
        }
        private void OnDisable()
        {
            if (initialized && stopOnDisable)
            {
                Stop();
            }
        }

        [Button]
        public void Play()
        {
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].Initialize();
                    components[i].Play();
                }
            }
        }
        public void PlaySingleComponent(int index)
        {
            if (components == null)
            {
                return;
            }
            if (index > components.Length - 1)
            {
                return;
            }
            components[index].Initialize();
            components[index].Play();
        }
        public void Play(FXPlayer componentToPlay)
        {
            if (componentToPlay == null)
            {
                return;
            }
            componentToPlay.Initialize();
            componentToPlay.Play();
        }
        [Button]
        public void Stop()
        {
            if (components == null)
            {
                return;
            }
            for (int i = 0; i < components.Length; i++)
            {
                components[i].Stop();
            }
        }
        [Button]
        public void ResetComponents()
        {
            if (components == null)
            {
                return;
            }
            for (int i = 0; i < components.Length; i++)
            {
                components[i].Stop();
            }
        }
    } 
}
