namespace Player
{
    public interface IPlayerState
    {
        public void Enter(PlayerController player);
        public void Update(PlayerController player, PlayerStateMachine stateMachine);
        public void Exit(PlayerController player);
    }
}