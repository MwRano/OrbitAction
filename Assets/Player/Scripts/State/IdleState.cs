#nullable enable
using UnityEngine;

namespace Player
{
    public class IdleState : IPlayerState
    {
        public void Enter(IPlayerContext playerContext)
        {
            playerContext.PlayerAnimator.SetTrigger(PlayerAnimationIds.IdleHash);
        }

        public void Update(IPlayerContext playerContext, PlayerStateMachine stateMachine)
        {
            // Deathへの遷移
            if (playerContext.IsDead)
            {
                stateMachine.TransitionTo(stateMachine.Death, playerContext);
            }

            if (!playerContext.IsGrounded)
            {
                switch (playerContext.Rigidbody.linearVelocityY)
                {
                    // Jumpへの遷移
                    case > 0:
                        stateMachine.TransitionTo(stateMachine.Jump, playerContext);
                        break;
                    // Fallへの遷移
                    case < 0:
                        stateMachine.TransitionTo(stateMachine.Fall, playerContext);
                        break;
                }
            }
            else
            {
                // Walkへの遷移
                if (Mathf.Abs(playerContext.Rigidbody.linearVelocityX) > 0f)
                {
                    stateMachine.TransitionTo(stateMachine.Walk, playerContext);
                }
            }
        }

        public void Exit(IPlayerContext playerContext)
        {
            playerContext.PlayerAnimator.ResetTrigger(PlayerAnimationIds.IdleHash);
        }
    }
}