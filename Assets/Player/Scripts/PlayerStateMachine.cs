#nullable enable

using VContainer;

namespace Player
{
    /// <summary>
    /// Playerの状態を管理するステートマシン
    /// </summary>
    public class PlayerStateMachine
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

        public void Initialize(IPlayerState startingState, PlayerController player)
        {
            _currentState = startingState;
            startingState.Enter(player);
        }

        public void TransitionTo(IPlayerState nextState, PlayerController player)
        {
            _currentState.Exit(player);
            _currentState = nextState;
            nextState.Enter(player);
        }

        public void Update(PlayerController player)
        {
            _currentState.Update(player, this);
        }
    }
}