#nullable enable

using Orbit.Core.StateMachine;
using Unity.Cinemachine;
using VContainer;

namespace Orbit.Player.State
{
    public class Death : IState<PlayerStateMachine>
    {
        private readonly CinemachineImpulseSource _impulseSource;
        private PlayerCore _player;

        [Inject]
        public Death(
            CinemachineImpulseSource impulseSource,
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
            _player.IsDead.Value = false;
        }
    }
}