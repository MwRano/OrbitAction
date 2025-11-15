#nullable enable
using UnityEngine;

namespace Player
{
    public class IdleState : IPlayerState
    {
        public void Enter(PlayerController player)
        {
            player.PlayerAnimator.SetTrigger(PlayerAnimationIds.IdleHash);
        }

        public void Update(PlayerController player, PlayerStateMachine stateMachine)
        {
            // Deathへの遷移
            if (player.IsDead)
            {
                stateMachine.TransitionTo(stateMachine.Death, player);
            }

            if (!player.IsGrounded)
            {
                switch (player.Rigidbody.linearVelocityY)
                {
                    // Jumpへの遷移
                    case > 0:
                        stateMachine.TransitionTo(stateMachine.Jump, player);
                        break;
                    // Fallへの遷移
                    case < 0:
                        stateMachine.TransitionTo(stateMachine.Fall, player);
                        break;
                }
            }
            else
            {
                // Walkへの遷移
                if (Mathf.Abs(player.Rigidbody.linearVelocityX) > 0f)
                {
                    stateMachine.TransitionTo(stateMachine.Walk, player);
                }
            }
        }

        public void Exit(PlayerController player)
        {
            player.PlayerAnimator.ResetTrigger(PlayerAnimationIds.IdleHash);
        }
    }
}