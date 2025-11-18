#nullable enable
using Planet;
using Player;
using Player.State;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
    public class GameLifeTimeScope : LifetimeScope
    {
        [SerializeField] PlayerParam playerParam = null!;
        [SerializeField] PlanetParams planetParams = null!;
        [SerializeField] PlayerEffectParams playerEffectParams = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            // Param
            builder.RegisterInstance(playerParam).AsImplementedInterfaces();
            builder.RegisterInstance(planetParams).AsImplementedInterfaces();
            builder.RegisterInstance(playerEffectParams).AsImplementedInterfaces();

            //Game
            builder.RegisterEntryPoint<GameClearDirector>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<CinemachineImpulseSource>();
            builder.RegisterComponentInHierarchy<AreaManager>();

            // Input
            builder.Register<InputSystemActions>(Lifetime.Scoped);
            builder.Register<PlayerInput>(Lifetime.Scoped);

            // Player
            builder.RegisterEntryPoint<PlayerStateMachine>(Lifetime.Scoped);
            builder.RegisterEntryPoint<PlayerMover>(Lifetime.Scoped);
            builder.RegisterEntryPoint<PlayerEffector>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<PlayerCore>();
            builder.Register<PlayerRespawner>(Lifetime.Scoped);
            builder.Register<PlayerInput>(Lifetime.Scoped);
            builder.Register<PlayerAimer>(Lifetime.Scoped);
            builder.Register<Idle>(Lifetime.Scoped);
            builder.Register<Walk>(Lifetime.Scoped);
            builder.Register<Jump>(Lifetime.Scoped);
            builder.Register<Fall>(Lifetime.Scoped);
            builder.Register<Death>(Lifetime.Scoped);

            // Planet
            builder.RegisterEntryPoint<PlanetStateMachine>(Lifetime.Scoped).AsSelf();
            builder.RegisterComponentInHierarchy<PlanetController>();
            builder.RegisterComponentInHierarchy<PlanetGuide>();
            builder.Register<DeployPositionCalculator>(Lifetime.Scoped);
            builder.Register<Hover>(Lifetime.Scoped);
            builder.Register<Follow>(Lifetime.Scoped);
            builder.Register<Travel>(Lifetime.Scoped);
            builder.Register<Deploy>(Lifetime.Scoped);
        }
    }
}