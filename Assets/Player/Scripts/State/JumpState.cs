#nullable enable
using UnityEngine;

namespace Player
{
    public class JumpState : IPlayerState
    {
        public void Enter(IPlayerContext playerContext)
        {
            playerContext.PlayerAnimator.SetTrigger(PlayerAnimationIds.JumpHash);
        }

        public void Update(IPlayerContext playerContext, PlayerStateMachine stateMachine)
        {
            // Deathへの遷移
            if(playerContext.IsDead) 
            {
                stateMachine.TransitionTo(stateMachine.Death, playerContext);
            }
            
            if (playerContext.IsGrounded)
            {
                // Walkへの遷移
                if (Mathf.Abs(playerContext.Rigidbody.linearVelocityX) > 0.1f)
                {
                    stateMachine.TransitionTo(stateMachine.Walk, playerContext);
                }
                // Idleへの遷移
                else
                {
                    stateMachine.TransitionTo(stateMachine.Idle, playerContext);
                }
            }

            // Fallへの遷移
            if (playerContext.Rigidbody.linearVelocityY < -0.1f)
            {
                stateMachine.TransitionTo(stateMachine.Fall, playerContext);
            }
        }

        public void Exit()
        {
        }
    }
}