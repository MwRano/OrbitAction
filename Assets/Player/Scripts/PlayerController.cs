#nullable enable
using UnityEngine;
using VContainer;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    private InputSystemActions _inputSystemActions = null!;
    private PlayerParam _playerParam = null!;
    private SpriteRenderer _spriteRenderer = null!;

    public Rigidbody2D RigidBody { get; private set; } = null!;
    public Animator PlayerAnimator{ get; private set; } = null!;
    public bool IsGrounded { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; } = null!;

    [Inject]
    public void Construct(
        InputSystemActions inputSystemActions,
        PlayerParam playerParam,
        PlayerStateMachine playerStateMachine)
    {
        _inputSystemActions = inputSystemActions;
        _playerParam = playerParam;
        StateMachine = playerStateMachine;

        RigidBody = GetComponent<Rigidbody2D>();
        PlayerAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        StateMachine.Initialize(playerStateMachine.Idle);

        // InputSystemへのメソッド登録
        _inputSystemActions.Player.Jump.performed += Jump;
        _inputSystemActions.Player.Enable();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        CheckGrounded();
        StateMachine.Update();
    }

    /// <summary>
    /// 接地判定を行うメソッド
    /// </summary>
    private void CheckGrounded()
    {
        Vector2 groundCheckPosition = (Vector2)transform.position + _playerParam.GroundCheckOffset;
        IsGrounded = Physics2D.OverlapCircle(groundCheckPosition, _playerParam.GroundCheckRadius, _playerParam.GroundLayer);
    }

    private void Move()
    {
        Vector2 moveInput = _inputSystemActions.Player.Move.ReadValue<Vector2>();
        RigidBody.linearVelocity = new Vector2(moveInput.x * _playerParam.MoveSpeed, RigidBody.linearVelocity.y);

        // 向きに応じてviewの反転                            
        if (moveInput.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = true;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!IsGrounded || context.canceled) return;

        RigidBody.linearVelocity = new Vector2(RigidBody.linearVelocity.x, 0);
        RigidBody.AddForce(Vector2.up * _playerParam.JumpForce, ForceMode2D.Impulse);
    }


}