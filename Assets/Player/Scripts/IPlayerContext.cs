using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public interface IPlayerContext
    {
        Transform PlayerTransform { get; }
        Rigidbody2D Rigidbody { get; }
        Animator PlayerAnimator { get; }
        bool IsGrounded { get; }
        bool IsFacingRight { get; }
        Vector2 LookingDirection { get; }
        bool IsGoalReached { get; }
        bool IsDead { get; set; }

        public void SetCanControl(bool value);
        public UniTask DisableGravityAsync();
    }
}