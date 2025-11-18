#nullable enable
using UnityEngine;
using Core.StateMachine;
using VContainer;

namespace Player.State
{
    public class Jump : IState<PlayerStateMachine>
    {
        private readonly PlayerCore _player;
        
        [Inject]
        public Jump(PlayerCore player)
        {
            _player = player;
        }

        public void Enter()
        {
            _player.Anim.SetTrigger(PlayerAnimationIds.JumpHash);
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
            }
            else
            {
                // Fallへの遷移
                if (_player.Rb.linearVelocityY < 0)
                {
                    stateMachine.TransitionTo(stateMachine.Fall);
                }
            }
        }

        public void Exit()
        {
        }
    }
}