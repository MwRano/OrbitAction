#nullable enable
using UnityEngine;
using VContainer;

public class IdleState : IPlayerState
{
    public void Enter(IPlayerContext playerContext)
    {
        playerContext.PlayerAnimator.SetTrigger(AnimatorParams.IdleHash);
    }

    public void Update(IPlayerContext playerContext, PlayerStateMachine stateMachine)
    {
        if (!playerContext.IsGrounded)
        {
            switch (playerContext.Rigidbody.linearVelocityY)
            {
                // Jumpへの遷移
                case > 0.1f:
                    stateMachine.TransitionTo(stateMachine.Jump, playerContext);
                    break;
                // Fallへの遷移
                case < -0.1f:
                    stateMachine.TransitionTo(stateMachine.Fall, playerContext);
                    break;
            }
        }
        
        // Walkへの遷移
        if (Mathf.Abs(playerContext.Rigidbody.linearVelocityX) > 0.1f)
        {
            stateMachine.TransitionTo(stateMachine.Walk, playerContext);
        }
    }

    public void Exit()
    {
        
    }

}