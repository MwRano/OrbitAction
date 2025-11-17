#nullable enable
using UnityEngine;

namespace Player.State
{
    public class WalkState : IPlayerState
    {
        private readonly PlayerCore _player;

        public WalkState(PlayerCore player)
        {
            _player = player;
        }

        public void Enter()
        {
            _player.Anim.SetTrigger(PlayerAnimationIds.WalkHash);
        }

        public void Update(PlayerStateMachine stateMachine)
        {
            if (_player.IsDead.CurrentValue)
                stateMachine.TransitionTo(stateMachine.Death);


            if (_player.IsGrounded.CurrentValue)
            {
                if (Mathf.Abs(_player.Rb.linearVelocityX) <= 0.2f)
                    stateMachine.TransitionTo(stateMachine.Idle);
            }
            else
            {
                switch (_player.Rb.linearVelocityY)
                {
                    case > 0.01f:
                        stateMachine.TransitionTo(stateMachine.Jump);
                        break;
                    case < -0.01f:
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