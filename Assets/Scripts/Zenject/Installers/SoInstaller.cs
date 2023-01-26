using UnityEngine;

namespace Zenject.Installers
{
    public class SoInstaller : MonoInstaller<SoInstaller>
    {
        [SerializeField] private DialogContainer dialogContainer;

        public override void InstallBindings()
        {
            Container.Bind<DialogContainer>().FromInstance(dialogContainer).AsSingle();
        }
    }
}