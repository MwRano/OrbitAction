#nullable enable
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Planet
{
    /// <summary>
    /// planetの状態を管理するクラス
    /// </summary>
    public class PlanetStateMachine : IInitializable, ITickable
    {
        [Inject]
        public PlanetStateMachine(
            HoverState hoverState,
            FollowState followState,
            TravelState travelState,
            DeployState deployState)
        {
            Hover = hoverState;
            Follow = followState;
            Travel = travelState;
            Deploy = deployState;
        }

        public IPlanetState CurrentState { get; private set; } = null!;
        public HoverState Hover { get; }
        public FollowState Follow { get; }
        public TravelState Travel { get; }
        public DeployState Deploy { get; }

        public void Initialize()
        {
            CurrentState = Follow;
            CurrentState.Enter();
        }

        public void TransitionTo(IPlanetState nextState)
        {
            CurrentState.Exit();
            CurrentState = nextState;
            nextState.Enter();
        }

        public void Tick()
        {
            CurrentState.Update(this);
        }
        
    }
}