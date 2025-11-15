#nullable enable

using Unity.Cinemachine;

namespace Player
{
    public class DeathState : IPlayerState
    {
        private readonly CinemachineImpulseSource _impulseSource;

        public DeathState(CinemachineImpulseSource impulseSource)
        {
            _impulseSource = impulseSource;
        }

        public void Enter(PlayerController player)
        {
            player.PlayerAnimator.SetTrigger(PlayerAnimationIds.DeathHash);
            player.IsDead = false;
            _impulseSource.GenerateImpulse();
        }

        public void Update(PlayerController player, PlayerStateMachine stateMachine)
        {
            if (player.IsGrounded)
            {
                stateMachine.TransitionTo(stateMachine.Idle, player);
            }
            else
            {
                // Fallへの遷移
                if (player.Rigidbody.linearVelocityY < 0f)
                {
                    stateMachine.TransitionTo(stateMachine.Fall, player);
                }
            }
        }

        public void Exit(PlayerController player)
        {
        }
    }
}