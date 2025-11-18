#nullable enable
using Orbit.Core.StateMachine;
using UnityEngine;
using VContainer;

namespace Orbit.Player.State
{
    public class Idle : IState<PlayerStateMachine>
    {
        private readonly PlayerCore _player;
        
        [Inject]
        public Idle(PlayerCore player)
        {
            _player = player;
        }

        public void Enter()
        {
            _player.Anim.SetTrigger(PlayerAnimationIds.IdleHash);
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
                if (Mathf.Abs(_player.Rb.linearVelocityX) > 0.1f)
                {
                    stateMachine.TransitionTo(stateMachine.Walk);
                }
            }
            else
            {
                switch (_player.Rb.linearVelocityY)
                {
                    // Jumpへの遷移
                    case > 0:
                        stateMachine.TransitionTo(stateMachine.Jump);
                        break;
                    // Fallへの遷移
                    case < 0:
                        stateMachine.TransitionTo(stateMachine.Fall);
                        break;
                }
            }
        }

        public void Exit()
        {
        }
    }
}