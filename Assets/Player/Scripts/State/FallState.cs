#nullable enable
using UnityEngine;

namespace Player
{
    public class FallState : IPlayerState
    {
        public void Enter(IPlayerContext playerContext)
        {
            playerContext.PlayerAnimator.SetTrigger(PlayerAnimationIds.FallHash);
        }

        public void Update(IPlayerContext playerContext, PlayerStateMachine stateMachine)
        {
            // Deathへの遷移
            if (playerContext.IsDead)
            {
                stateMachine.TransitionTo(stateMachine.Death, playerContext);
            }

            if (playerContext.IsGrounded)
            {
                // Walkへの遷移
                if (Mathf.Abs(playerContext.Rigidbody.linearVelocityX) > 0)
                {
                    stateMachine.TransitionTo(stateMachine.Walk, playerContext);
                }
                // Idleへの遷移
                else
                {
                    stateMachine.TransitionTo(stateMachine.Idle, playerContext);
                }
            }

            // Jumpへの遷移
            if (playerContext.Rigidbody.linearVelocityY > 0)
            {
                stateMachine.TransitionTo(stateMachine.Jump, playerContext);
            }
        }

        public void Exit(IPlayerContext playerContext)
        {
            playerContext.PlayerAnimator.ResetTrigger(PlayerAnimationIds.FallHash);
        }
    }
}