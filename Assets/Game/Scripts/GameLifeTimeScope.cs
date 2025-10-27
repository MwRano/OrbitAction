#nullable enable
using Planet;
using Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
    public class GameLifeTimeScope : LifetimeScope
    {
        [SerializeField] PlayerParam playerParam = null!;
        [SerializeField] PlanetParams planetParams = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            // Param
            builder.RegisterInstance(playerParam).AsImplementedInterfaces();
            builder.RegisterInstance(planetParams).AsImplementedInterfaces();

            //Game
            builder.RegisterEntryPoint<GameClearDirector>(Lifetime.Scoped);

            // Input
            builder.Register<InputSystemActions>(Lifetime.Scoped);

            // Player
            builder.RegisterComponentInHierarchy<PlayerController>().As<IPlayerContext>().AsSelf();
            builder.Register<PlayerStateMachine>(Lifetime.Scoped);
            builder.Register<IdleState>(Lifetime.Scoped);
            builder.Register<WalkState>(Lifetime.Scoped);
            builder.Register<JumpState>(Lifetime.Scoped);
            builder.Register<FallState>(Lifetime.Scoped);
            builder.Register<DeathState>(Lifetime.Scoped);

            // Planet
            builder.RegisterComponentInHierarchy<PlanetController>().As<IPlanetContext>().AsSelf();
            builder.Register<PlanetStateMachine>(Lifetime.Scoped);
            builder.Register<HoverState>(Lifetime.Scoped);
            builder.Register<FollowState>(Lifetime.Scoped);
            builder.Register<TravelState>(Lifetime.Scoped);
            builder.Register<DeployState>(Lifetime.Scoped);
        }
    }
}