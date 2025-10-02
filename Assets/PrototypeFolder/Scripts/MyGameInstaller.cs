using UnityEngine;
using Zenject;

namespace GeoMovement
{
    public class MyGameInstaller : MonoInstaller
    {
        [SerializeField] private GameSettings _settings;
        private string _box = "Prefabs/Box";
        public override void InstallBindings()
        {
            Container.BindFactory<BoxView.Settings, BoxView, BoxView.Factory>()
              .FromMonoPoolableMemoryPool(
                  x => x.WithInitialSize(9)
                      .WithMaxSize(10)
                      .FromComponentInNewPrefabResource(_box)
                      .UnderTransformGroup("BoxPool"));

            Container.Bind<GameSettings>().FromInstance(_settings).NonLazy();

            Container.Bind<DataReaderService>().AsSingle().NonLazy();
            Container.Bind<SpawnBoxService>().AsSingle().NonLazy();
        }
    }
}