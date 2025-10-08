using UnityEngine;
using Zenject;

namespace GeoMovement
{
    public class MyGameInstaller : MonoInstaller
    {
        [SerializeField] private GameSettings   _settings;
        private string                          _box = "Prefabs/Box";
        private string                          _player = "Prefabs/Player";
        public override void InstallBindings()
        {
            Container.Bind<PlayerView>().FromComponentInNewPrefabResource(_player).AsSingle().NonLazy();

            Container.BindFactory<BoxView.Settings, BoxView, BoxView.Factory>()
            .FromMonoPoolableMemoryPool(x => x.WithInitialSize(9)
            .WithMaxSize(10)
            .FromComponentInNewPrefabResource(_box)
            .UnderTransformGroup("BoxPool")
            );

            Container.Bind<GameSettings>().FromInstance(_settings).NonLazy();
            Container.Bind<DataReaderService>().AsSingle().NonLazy();
            Container.Bind<SpawnBoxService>().AsSingle().NonLazy();
            Container.Bind<UpdateBoxService>().AsSingle().NonLazy();
            Container.Bind<MovePlayerService>().AsSingle().NonLazy();
        }
    }
}