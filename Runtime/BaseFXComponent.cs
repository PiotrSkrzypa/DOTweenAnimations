using System;
using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public abstract class BaseFXComponent
    {
        
        public FXTiming Timing;

        CancellationTokenSource cts;

        public virtual void Initialize()
        {

        }
        [Button]
        public async UniTask Play()
        {
            CancellationTokenCleanUp();
            cts = new CancellationTokenSource();
            try
            {
                await UniTask.Delay((int)( Timing.InitialDelay * 1000 ), cancellationToken: cts.Token);
                Timing.IsRunning = true;
                await PlayInternal(cancellationToken: cts.Token);
                Timing.IsRunning = false;
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
        [Button]
        public void Stop()
        {
            CancellationTokenCleanUp();
            if (Timing.IsRunning)
            {
                Timing.IsRunning = false;
                StopInternal();
            }
        }
        protected virtual void StopInternal()
        {
            
        }
        [Button]
        public void Reset()
        {
            CancellationTokenCleanUp();
            if (Timing.IsRunning)
            {
                Timing.IsRunning = false;
            }
            ResetInternal();
        }
        protected virtual void ResetInternal()
        {
        }

        protected virtual UniTask PlayInternal(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        private void CancellationTokenCleanUp()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

    }
}