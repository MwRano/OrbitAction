#nullable enable
using Orbit.Planet;
using Orbit.Planet.State;
using Orbit.Player;
using Orbit.Player.State;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Orbit.Game
{
    public class GameLifeTimeScope : LifetimeScope
    {
        [SerializeField] private PlayerParam playerParam = null!;
        [SerializeField] private PlanetParams planetParams = null!;
        [SerializeField] private PlayerEffectParams playerEffectParams = null!;
        [SerializeField] private FadeParams fadeParams = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            // Param
            builder.RegisterInstance(playerParam).AsImplementedInterfaces();
            builder.RegisterInstance(planetParams).AsImplementedInterfaces();
            builder.RegisterInstance(playerEffectParams).AsImplementedInterfaces();
            builder.RegisterInstance(fadeParams).AsImplementedInterfaces();

            //Game
            builder.RegisterEntryPoint<GameClearDirector>(Lifetime.Scoped);
            builder.Register<ScreenFader>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<CinemachineImpulseSource>();
            builder.RegisterComponentInHierarchy<SoundManager>();

            // Input
            builder.Register<InputSystemActions>(Lifetime.Scoped);
            builder.Register<PlayerInput>(Lifetime.Scoped);
            builder.Register<PlanetInput>(Lifetime.Scoped);

            // Player
            builder.RegisterEntryPoint<PlayerStateMachine>(Lifetime.Scoped);
            builder.RegisterEntryPoint<PlayerEffector>(Lifetime.Scoped);
            builder.RegisterEntryPoint<PlayerDeathPresenter>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<PlayerCore>();
            builder.Register<PlayerMover>(Lifetime.Scoped);
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
            builder.RegisterEntryPoint<PlanetSkill>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<PlanetCore>();
            builder.RegisterComponentInHierarchy<PlanetGuide>();
            builder.Register<DeployPositionCalculator>(Lifetime.Scoped);
            builder.Register<Hover>(Lifetime.Scoped);
            builder.Register<Follow>(Lifetime.Scoped);
            builder.Register<Travel>(Lifetime.Scoped);
            builder.Register<Deploy>(Lifetime.Scoped);
        }
    }
}