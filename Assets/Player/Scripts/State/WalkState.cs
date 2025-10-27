#nullable enable
using UnityEngine;

namespace Player
{
    public class WalkState : IPlayerState
    {
        public void Enter(IPlayerContext playerContext)
        {
            playerContext.PlayerAnimator.SetTrigger(PlayerAnimationIds.WalkHash);
        }

        public void Update(IPlayerContext playerContext, PlayerStateMachine stateMachine)
        {
            // Deathへの遷移
            if(playerContext.IsDead) 
            {
                stateMachine.TransitionTo(stateMachine.Death, playerContext);
            }
            
            if (!playerContext.IsGrounded)
            {
                switch (playerContext.Rigidbody.linearVelocityY)
                {
                    // Jumpへの遷移
                    case > 0.1f:
                        stateMachine.TransitionTo(stateMachine.Jump, playerContext);
                        break;
                    // Fallへの遷移
                    case < -0.1f:
                        stateMachine.TransitionTo(stateMachine.Fall, playerContext);
                        break;
                }
            }

            // idleへの遷移
            if (Mathf.Abs(playerContext.Rigidbody.linearVelocityX) < 0.1f)
            {
                stateMachine.TransitionTo(stateMachine.Idle, playerContext);
            }
        }

        public void Exit()
        {
        }
    }
}