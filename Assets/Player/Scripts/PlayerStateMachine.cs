#nullable enable

/// <summary>
/// Playerの状態を管理するステートマシン
/// </summary>
public class PlayerStateMachine
{
    public IPlayerState? CurrentState { get; private set; }


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