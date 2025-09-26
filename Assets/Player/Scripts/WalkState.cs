#nullable enable
using UnityEngine;
using VContainer;

public class WalkState : IPlayerState
{
    private readonly PlayerController _player;

    [Inject]
    public WalkState(PlayerController playerController)
    {
        _player = playerController;
    }

    public void Enter()
    {

    }

    public void Update()
    {
        if (_player.IsGrounded == false)
        {
            // Jumpへの遷移
            if (_player.RigidBody.linearVelocityY > 0.1f)
            {
                _player.StateMachine.TransitionTo(_player.StateMachine.Jump);
            }
            // Fallへの遷移
            else if (_player.RigidBody.linearVelocityY < -0.1f)
            {
                _player.StateMachine.TransitionTo(_player.StateMachine.Fall);
            }
        }

        // idleへの遷移
        if (Mathf.Abs(_player.RigidBody.linearVelocityX) <= 0.1f)
        {
            _player.StateMachine.TransitionTo(_player.StateMachine.Idle);
        }
    }

    public void Exit()
    {
        
    }

}