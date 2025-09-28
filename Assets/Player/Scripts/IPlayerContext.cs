using UnityEngine;

public interface IPlayerContext
{
    Rigidbody2D Rigidbody { get; }
    Animator PlayerAnimator { get; }
    bool IsGrounded { get; }
}
