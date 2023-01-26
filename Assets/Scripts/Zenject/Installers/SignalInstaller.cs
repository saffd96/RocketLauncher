using UI;
using Zenject.Extras;

namespace Zenject.Installers
{
    public class SignalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<Signals.CloseAllDialogsSignal>();
            Container.DeclareSignal<Signals.OnNewDialogShowSignal>();
            Container.DeclareSignal<Signals.ResetGameSignal>();
            Container.DeclareSignal<Signals.FirstTouchSignal>();
            Container.DeclareSignal<Signals.SpawnPlatformSignal>();
            Container.DeclareSignal<Signals.AddPointsSignal>();
            Container.DeclareSignal<Signals.UpdateUIPointsSignal>();
            Container.DeclareSignal<Signals.SendIsTouchedSignal>();
            Container.DeclareSignal<Signals.PlayerDeathSignal>();
            
            Container.DeclareShowPopUpSignal<Signals.ShowLoseDialogSignal, LoseDialog>();
        }
    }
}