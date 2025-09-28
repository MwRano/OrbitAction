public interface IPlayerState
{
    public void Enter(IPlayerContext playerContext);
    public void Update(IPlayerContext playerContext, PlayerStateMachine stateMachine);
    public void Exit();
}

