using UnityEngine;

public interface IPlayerContext
{
    Transform PlayerTransform { get; }
    Rigidbody2D Rigidbody { get; }
    Animator PlayerAnimator { get; }
    bool IsGrounded { get; }
    
    Vector2 CurrentPosition { get; }
    Vector2 CurrentVelocity { get; }
    bool IsFacingRight { get; }
    Vector2 LookingDirection { get; }

    public void SetCanControl(bool value);

}
