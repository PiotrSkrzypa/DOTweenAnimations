using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public abstract class BaseFXComponent
    {
        [SerializeField]private float delay;
        protected float Delay => delay;
        CancellationTokenSource cts;

        public virtual void Initialize()
        {

        }
        public virtual void Play()
        {
            CancellationTokenCleanUp();
            cts = new CancellationTokenSource();
            PlayAfterDelay(cts.Token).Forget();
        }

        async UniTask PlayAfterDelay(CancellationToken token)
        {
            try
            {
                await UniTask.Delay((int)( delay * 1000 ), cancellationToken: token);
                PlayInternal();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Task was canceled");
            }
            finally
            {
                CancellationTokenCleanUp();
            }
        }
        public virtual void Stop()
        {
            CancellationTokenCleanUp();
        }
        public virtual void Reset()
        {
            CancellationTokenCleanUp();
        }

        protected virtual void PlayInternal()
        {
        }

        private void CancellationTokenCleanUp()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

    }
}