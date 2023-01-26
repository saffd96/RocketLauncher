using Cysharp.Threading.Tasks;
using UniRx;

namespace Zenject.Extras
{
    public static class ZenjectSignalsExtensions
    {
        public static void DeclareShowPopUpSignal<TSignal, TDialog>(this DiContainer container)
            where TDialog : Dialog<TSignal>
        {
            container.DeclareSignal<TSignal>();

            container.BindSignal<TSignal>()
                .ToMethod<DialogManager>((m, s) => m.ShowDialog<TDialog, TSignal>(s))
                .FromResolve();
        }

        public static async UniTask<TSignal> WaitForSignalAsync<TSignal>(this SignalBus signalBus)
            => await signalBus.GetStream<TSignal>().ToUniTask(true);

        public static async UniTask WaitForAnySignalAsync<TSignal1, TSignal2>(this SignalBus signalBus)
        {
            var task1 = signalBus.WaitForSignalAsync<TSignal1>();
            var task2 = signalBus.WaitForSignalAsync<TSignal2>();

            await UniTask.WhenAny(task1, task2);
        }
    
        public static SignalBus OnlyOnceOn<TSignal>(this SignalBus signalBus, System.Action<TSignal> action, System.Func<TSignal, bool> where = null)
        {
            if (where != null)
            {
                signalBus.GetStream<TSignal>().Where(where).First().Subscribe(action);
            }
            else
            {
                signalBus.GetStream<TSignal>().First().Subscribe(action);
            }

            return signalBus;
        }
    
        public static SignalBus OnlyOnceOn<TSignal>(this SignalBus signalBus, System.Action action, System.Func<TSignal, bool> where = null)
        {
            if (where != null)
            {
                signalBus.GetStream<TSignal>().Where(where).First().Subscribe(_ => action());
            }
            else
            {
                signalBus.GetStream<TSignal>().First().Subscribe(_ => action());
            }

            return signalBus;
        }
    }
}