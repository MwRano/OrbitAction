#nullable enable
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifeTimeScope : LifetimeScope
{
    [SerializeField] PlayerParam playerParam = null!;

    protected override void Configure(IContainerBuilder builder)
    {
        // Param
        builder.RegisterInstance(playerParam).AsImplementedInterfaces();

        // Player
        builder.RegisterComponentInHierarchy<PlayerController>();
        builder.Register<PlayerStateMachine>(Lifetime.Scoped);
        builder.Register<IdleState>(Lifetime.Scoped);
        builder.Register<WalkState>(Lifetime.Scoped);
        builder.Register<JumpState>(Lifetime.Scoped);
        builder.Register<FallState>(Lifetime.Scoped);
    
    }
}
