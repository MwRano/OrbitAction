#nullable enable
using UnityEngine;
using VContainer;

public class FallState : IPlayerState
{
    private readonly PlayerController _player;

    [Inject]
    public FallState(PlayerController playerController)
    {
        _player = playerController;
    }

    public void Enter()
    {
        _player.PlayerAnimator.SetTrigger(AnimatorParams.FallHash);
    }

    public void Update()
    {
        if (_player.IsGrounded)
        {
            // Walkへの遷移
            if (Mathf.Abs(_player.RigidBody.linearVelocityX) > 0.1f)
            {
                _player.StateMachine.TransitionTo(_player.StateMachine.Walk);
            }
            // Idleへの遷移
            else
            {
                _player.StateMachine.TransitionTo(_player.StateMachine.Idle);
            }
        }

        // Jumpへの遷移
        if (_player.RigidBody.linearVelocityY > 0.1f)
        {
            _player.StateMachine.TransitionTo(_player.StateMachine.Jump);
        }
    }

    public void Exit()
    {
        
    }

}