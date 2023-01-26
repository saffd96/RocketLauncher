using UnityEngine;

namespace Zenject.Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private DialogManager dialogManager;

        public override void InstallBindings()
        {
            Container.Bind<DialogManager>().FromInstance(dialogManager).AsSingle();
        }
    }
}