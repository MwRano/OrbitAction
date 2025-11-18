#nullable enable
using UnityEngine;
using Core.StateMachine;
using VContainer;

namespace Player.State
{
    public class Fall : IState<PlayerStateMachine>
    {
        private readonly PlayerCore _player;
        
        [Inject]
        public Fall(PlayerCore player)
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