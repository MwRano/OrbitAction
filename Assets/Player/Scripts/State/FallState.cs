#nullable enable
using UnityEngine;

public class FallState : IPlayerState
{
    public void Enter(IPlayerContext playerContext)
    {
        playerContext.PlayerAnimator.SetTrigger(AnimatorParams.FallHash);
    }

    public void Update(IPlayerContext playerContext, PlayerStateMachine stateMachine)
    {
        if (playerContext.IsGrounded)
        {
            // Walkへの遷移
            if (Mathf.Abs(playerContext.Rigidbody.linearVelocityX) > 0.1f)
            {
                stateMachine.TransitionTo(stateMachine.Walk, playerContext);
            }
            // Idleへの遷移
            else
            {
                stateMachine.TransitionTo(stateMachine.Idle, playerContext);
            }
        }

        // Jumpへの遷移
        if (playerContext.Rigidbody.linearVelocityY > 0.1f)
        {
            stateMachine.TransitionTo(stateMachine.Jump, playerContext);
        }
    }

    public void Exit()
    {
        
    }

}