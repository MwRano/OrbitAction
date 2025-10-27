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

        public void Initialize(IPlayerState startingState, IPlayerContext playerContext)
        {
            _currentState = startingState;
            startingState.Enter(playerContext);
        }

        public void TransitionTo(IPlayerState nextState, IPlayerContext playerContext)
        {
            _currentState.Exit();
            _currentState = nextState;
            nextState.Enter(playerContext);
        }

        public void Update(IPlayerContext playerContext)
        {
            _currentState.Update(playerContext, this);
        }
    }
}