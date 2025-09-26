#nullable enable

using VContainer;

/// <summary>
/// Playerの状態を管理するステートマシン
/// </summary>
public class PlayerStateMachine
{
    public IPlayerState? CurrentState { get; private set; }
    public IdleState Idle { get; }
    public WalkState Walk { get; }
    public JumpState Jump { get; }
    public FallState Fall { get; }

    [Inject]
    public PlayerStateMachine(
        IdleState idleState,
        WalkState walkState,
        JumpState jumpState,
        FallState fallState)
    {
        Idle = idleState;
        Walk = walkState;
        Jump = jumpState;
        Fall = fallState;
    }

    public void Initialize(IPlayerState startingState)
    {
        CurrentState = startingState;
        startingState.Enter();
    }

    public void TransitionTo(IPlayerState nextState)
    {
        CurrentState?.Exit();
        CurrentState = nextState;
        nextState.Enter();
    }

    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}