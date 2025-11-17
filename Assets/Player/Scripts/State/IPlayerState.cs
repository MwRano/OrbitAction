namespace Player.State
{
    public interface IPlayerState
    {
        public void Enter();
        public void Update(PlayerStateMachine stateMachine);
        public void Exit();
    }
}