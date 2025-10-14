#nullable enable
using UnityEngine;
using VContainer;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, IPlayerContext
{
    private bool _canControl = true;
    private InputSystemActions _inputSystemActions = null!;
    private PlayerParam _playerParams = null!;
    private SpriteRenderer _spriteRenderer = null!;
    private PlayerStateMachine _stateMachine = null!;

    void Update()
    {
        CurrentPosition = transform.position;
        CurrentVelocity = Rigidbody.linearVelocity;
        CheckGrounded();
        _stateMachine.Update(this);
    }

    void FixedUpdate()
    {
        if (_canControl) Move();
        Look();
    }

    public Rigidbody2D Rigidbody { get; private set; } = null!;
    public Animator PlayerAnimator { get; private set; } = null!;
    public bool IsGrounded { get; private set; }
    public bool IsFacingRight { get; private set; }
    public Vector2 LookingDirection { get; private set; }
    public Vector2 CurrentPosition { get; private set; }
    public Vector2 CurrentVelocity { get; private set; }

    /// <summary>
    /// playerの操作が可能かどうかを設定するメソッド
    /// </summary>
    /// <param name="value"></param>
    public void SetCanControl(bool value)
    {
        _canControl = value;
    }

    [Inject]
    public void Construct(
        InputSystemActions inputSystemActions,
        PlayerParam playerParams,
        PlayerStateMachine playerStateMachine)
    {
        _inputSystemActions = inputSystemActions;
        _playerParams = playerParams;
        _stateMachine = playerStateMachine;

        // コンポーネントの取得
        Rigidbody = GetComponent<Rigidbody2D>();
        PlayerAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // InputSystemへのメソッド登録
        _inputSystemActions.Player.Jump.performed += Jump;
        _inputSystemActions.Player.Enable();
        IsFacingRight = _spriteRenderer.flipX;

        // SMの初期化
        _stateMachine.Initialize(playerStateMachine.Idle, this);
    }

    /// <summary>
    /// 接地判定を行うメソッド
    /// </summary>
    private void CheckGrounded()
    {
        Vector2 groundCheckPosition = (Vector2)transform.position + _playerParams.GroundCheckOffset;
        IsGrounded = Physics2D.OverlapCircle(groundCheckPosition, _playerParams.GroundCheckRadius,
            _playerParams.GroundLayer);
    }

    private void Move()
    {
        Vector2 moveInput = _inputSystemActions.Player.Move.ReadValue<Vector2>();
        if (Mathf.Abs(moveInput.x) < 0.01f) return;
        Rigidbody.linearVelocity = new Vector2(moveInput.x * _playerParams.MoveSpeed, Rigidbody.linearVelocity.y);

        // 向きに応じてviewの反転    
        _spriteRenderer.flipX = moveInput.x switch
        {
            > 0 => false,
            < 0 => true,
            _ => _spriteRenderer.flipX
        };
        IsFacingRight = !_spriteRenderer.flipX;
    }

    private void Look()
    {
        Vector2 lookInput = _inputSystemActions.Player.Look.ReadValue<Vector2>();
        if (lookInput.sqrMagnitude > 0.1f) LookingDirection = lookInput.normalized;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!IsGrounded || context.canceled) return;

        Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocity.x, 0);
        Rigidbody.AddForce(Vector2.up * _playerParams.JumpForce, ForceMode2D.Impulse);
    }
}