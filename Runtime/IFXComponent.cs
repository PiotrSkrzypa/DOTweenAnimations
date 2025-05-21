using Cysharp.Threading.Tasks;

namespace PSkrzypa.UnityFX
{
    public interface IFXComponent
    {
        FXTiming Timing { get; }
        void Initialize();
        UniTask Play();
        void Reset();
        void Stop();
    }
}