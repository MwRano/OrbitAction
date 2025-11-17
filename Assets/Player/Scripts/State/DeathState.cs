#nullable enable

using Unity.Cinemachine;

namespace Player.State
{
    public class DeathState : IPlayerState
    {
        private readonly CinemachineImpulseSource _impulseSource;
        private PlayerCore _player;

        public DeathState(CinemachineImpulseSource impulseSource,
            PlayerCore player)
        {
            _impulseSource = impulseSource;
            _player = player;
        }

        public void Enter()
        {
            _player.Anim.SetTrigger(PlayerAnimationIds.DeathHash);
            _impulseSource.GenerateImpulse();
        }

        public void Update(PlayerStateMachine stateMachine)
        {
            if (_player.IsGrounded.CurrentValue)
            {
                stateMachine.TransitionTo(stateMachine.Idle);
            }
            else
            {
                // Fallへの遷移
                if (_player.Rb.linearVelocityY < 0f)
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