#nullable enable
using UnityEngine;

namespace Player.State
{
    public class FallState : IPlayerState
    {
        private readonly PlayerCore _player;

        public FallState(PlayerCore player)
        {
            _player = player;
        }

        public void Enter()
        {
            _player.Anim.SetTrigger(PlayerAnimationIds.FallHash);
        }

        public void Update(PlayerStateMachine stateMachine)
        {
            // Deathへの遷移
            if (_player.IsDead.CurrentValue)
            {
                stateMachine.TransitionTo(stateMachine.Death);
            }

            if (_player.IsGrounded.CurrentValue)
            {
                // Walkへの遷移
                if (Mathf.Abs(_player.Rb.linearVelocityX) > 0)
                {
                    stateMachine.TransitionTo(stateMachine.Walk);
                }
                // Idleへの遷移
                else
                {
                    stateMachine.TransitionTo(stateMachine.Idle);
                }
            }
            else
            {
                // Jumpへの遷移
                if (_player.Rb.linearVelocityY > 0)
                {
                    stateMachine.TransitionTo(stateMachine.Jump);
                }
            }
        }

        public void Exit()
        {
        }
    }
}