#nullable enable
using UnityEngine;

namespace Player
{
    public class JumpState : IPlayerState
    {
        public void Enter(PlayerController playerContext)
        {
            playerContext.PlayerAnimator.SetTrigger(PlayerAnimationIds.JumpHash);
        }

        public void Update(PlayerController playerContext, PlayerStateMachine stateMachine)
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
            }
            else
            {
                // Fallへの遷移
                if (playerContext.Rigidbody.linearVelocityY < 0)
                {
                    stateMachine.TransitionTo(stateMachine.Fall, playerContext);
                }
            }
        }

        public void Exit(PlayerController playerContext)
        {
            playerContext.PlayerAnimator.ResetTrigger(PlayerAnimationIds.JumpHash);
        }
    }
}