#nullable enable

namespace Player
{
    public class DeathState : IPlayerState
    {
        public void Enter(IPlayerContext playerContext)
        {
            playerContext.PlayerAnimator.SetTrigger(PlayerAnimationIds.DeathHash);
            playerContext.IsDead = false;
        }

        public void Update(IPlayerContext playerContext, PlayerStateMachine stateMachine)
        {
            if (playerContext.IsGrounded)
            {
                stateMachine.TransitionTo(stateMachine.Idle, playerContext);
            }
            else
            {
                // Fallへの遷移
                if (playerContext.Rigidbody.linearVelocityY < 0f)
                {
                    stateMachine.TransitionTo(stateMachine.Fall, playerContext);
                }
            }
        }

        public void Exit(IPlayerContext playerContext)
        {
            playerContext.PlayerAnimator.ResetTrigger(PlayerAnimationIds.DeathHash);
        }
    }
}