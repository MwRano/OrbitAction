#nullable enable

namespace Player
{
    public class DeathState : IPlayerState
    {
        public void Enter(IPlayerContext playerContext)
        {
            playerContext.PlayerAnimator.SetTrigger(PlayerAnimationIds.DeathHash);
        }

        public void Update(IPlayerContext playerContext, PlayerStateMachine stateMachine)
        {
            stateMachine.TransitionTo(stateMachine.Idle, playerContext);
        }

        public void Exit()
        {
            
        }
    }
}