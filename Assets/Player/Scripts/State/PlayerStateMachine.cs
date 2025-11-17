#nullable enable

using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Player.State
{
    /// <summary>
    /// Playerの状態を管理するステートマシン
    /// </summary>
    public class PlayerStateMachine : IInitializable, ITickable
    {
        private IPlayerState _currentState = null!;

        [Inject]
        public PlayerStateMachine(
            IdleState idleState,
            WalkState walkState,
            JumpState jumpState,
            FallState fallState,
            DeathState deathState)
        {
            Idle = idleState;
            Walk = walkState;
            Jump = jumpState;
            Fall = fallState;
            Death = deathState;
        }

        public IdleState Idle { get; }
        public WalkState Walk { get; }
        public JumpState Jump { get; }
        public FallState Fall { get; }
        public DeathState Death { get; }

        public void Initialize()
        {
            _currentState = Idle;
            _currentState.Enter();
        }

        public void Tick()
        {
            _currentState.Update(this);
        }

        public void TransitionTo(IPlayerState nextState)
        {
            _currentState.Exit();
            _currentState = nextState;
            nextState.Enter();
        }
    }
}