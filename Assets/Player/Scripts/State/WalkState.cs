#nullable enable
using UnityEngine;

namespace Player
{
    public class WalkState : IPlayerState
    {
        const float VerticalThreshold = 0.1f;
        const float HorizontalIdleThreshold = 0.1f;

        public void Enter(PlayerController playerContext)
        {
            playerContext.PlayerAnimator.SetTrigger(PlayerAnimationIds.WalkHash);
        }

        public void Update(PlayerController playerContext, PlayerStateMachine stateMachine)
        {
            if (ShouldTransitionToDeath(playerContext))
            {
                stateMachine.TransitionTo(stateMachine.Death, playerContext);
                return;
            }

            if (TryHandleAirTransition(playerContext, stateMachine)) return;

            if (TryHandleIdleTransition(playerContext, stateMachine)) return;
        }

        public void Exit(PlayerController playerContext)
        {
            playerContext.PlayerAnimator.ResetTrigger(PlayerAnimationIds.WalkHash);
        }

        private static bool ShouldTransitionToDeath(PlayerController ctx) => ctx.IsDead;

        private static bool TryHandleAirTransition(PlayerController ctx, PlayerStateMachine sm)
        {
            if (ctx.IsGrounded) return false;

            switch (ctx.Rigidbody.linearVelocityY)
            {
                case > VerticalThreshold:
                    sm.TransitionTo(sm.Jump, ctx);
                    return true;
                case < -VerticalThreshold:
                    sm.TransitionTo(sm.Fall, ctx);
                    return true;
                default:
                    return false;
            }
        }

        private static bool TryHandleIdleTransition(PlayerController ctx, PlayerStateMachine sm)
        {
            if (Mathf.Abs(ctx.Rigidbody.linearVelocityX) < HorizontalIdleThreshold)
            {
                sm.TransitionTo(sm.Idle, ctx);
                return true;
            }

            return false;
        }
    }
}