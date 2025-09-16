using Managers;
using Managers.Board;
using Signals;
using Tile;
using UnityEngine;
using Zenject;

namespace Installers.Core
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Pool")]
        [SerializeField] private GameObject tilePrefab;

        [SerializeField] private Transform tileSpawnTransform;
        public override void InstallBindings()
        {
            Container.Bind<BoardController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<BoardInputs>().AsSingle();
            Container.Bind<SwipeDetection>().AsSingle();
            
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<Moving>();
            Container.DeclareSignal<GenerateLevel>();
            Container.DeclareSignal<RestartGame>();
            Container.DeclareSignal<Win>();
            
            Container.Bind<BoardModel>().AsSingle();
            
            Container.Bind<TileModel>().AsTransient();

            Container.BindMemoryPool<TileController, TileController.Pool>()
                .WithInitialSize(16)
                .FromComponentInNewPrefab(tilePrefab)
                .UnderTransform(tileSpawnTransform);
        }
    }
}