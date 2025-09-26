#nullable enable
using UnityEngine;
using VContainer;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class PlayerController : MonoBehaviour
{
    private InputSystemActions _inputSystemActions = null!;
    private PlayerParam _playerParam = null!;
    private PlayerStateMachine _playerStateMachine = null!;
    private IdleState _idleState = null!;
    private Rigidbody2D _rb = null!;
    private SpriteRenderer _spriteRenderer = null!;

    private Vector2 _moveInput;
    private bool _isGrounded;

    public bool IsGrounded => _isGrounded;

    [Inject]
    public void Construct(
        InputSystemActions inputSystemActions,
        PlayerParam playerParam,
        PlayerStateMachine playerStateMachine,
        IdleState idleState)
    {
        _inputSystemActions = inputSystemActions;
        _playerParam = playerParam;
        _playerStateMachine = playerStateMachine;
        _idleState = idleState;

        _rb = gameObject.GetComponent<Rigidbody2D>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        _playerStateMachine.Initialize(_idleState);
    }

    void Update()
    {
        CheckGrounded();
        Move();
        if (_isGrounded) Jump();

        _playerStateMachine.Update();
    }

    /// <summary>
    /// 接地判定を行うメソッド
    /// </summary>
    private void CheckGrounded()
    {
        Vector2 groundCheckPosition = (Vector2)transform.position + _playerParam.GroundCheckOffset;
        _isGrounded = Physics2D.OverlapCircle(groundCheckPosition, _playerParam.GroundCheckRadius, _playerParam.GroundLayer);
    }

    private void Move()
    {
        _moveInput = _inputSystemActions.Player.Move.ReadValue<Vector2>();
        _rb.linearVelocity = new Vector2(_moveInput.x * _playerParam.MoveSpeed, _rb.linearVelocity.y);

        // 向きに応じてviewの反転                            
        if (_moveInput.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = true;
        }
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
        _rb.AddForce(Vector2.up * _playerParam.JumpForce, ForceMode2D.Impulse);
    }


}