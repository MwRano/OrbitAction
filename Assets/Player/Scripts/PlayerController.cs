#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour, IPlayerContext
    {
        private float _baseGravityScale;
        private bool _canControl = true;
        private InputSystemActions _inputSystemActions = null!;
        private PlayerParam _playerParams = null!;
        private SpriteRenderer _spriteRenderer = null!;
        private PlayerStateMachine _stateMachine = null!;

        private void Update()
        {
            CheckGrounded();
            _stateMachine.Update(this);
        }

        private void FixedUpdate()
        {
            if (_canControl) Move();
            Look();
        }

        // 接触判定
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Goal")) return;
            IsGoalReached = true;
        }

        public bool IsGoalReached { get; private set; } = false;

        public Transform PlayerTransform { get; private set; } = null!;
        public Rigidbody2D Rigidbody { get; private set; } = null!;
        public Animator PlayerAnimator { get; private set; } = null!;
        public bool IsGrounded { get; private set; }
        public bool IsFacingRight { get; private set; }
        public Vector2 LookingDirection { get; private set; }

        /// <summary>
        /// playerの操作が可能かどうかを設定するメソッド
        /// </summary>
        /// <param name="value"></param>
        public void SetCanControl(bool value)
        {
            _canControl = value;
        }

        /// <summary>
        /// 重力を一時的に無効化するメソッド
        /// </summary>
        public void DisableGravity()
        {
            _baseGravityScale = Rigidbody.gravityScale;
            Rigidbody.gravityScale = 0;
            Rigidbody.linearVelocity = Vector2.zero;
            Invoke(nameof(SetGravity), 1f); //HACK: 遅延処理は要変更
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
            PlayerTransform = transform;

            // InputSystemへのメソッド登録
            _inputSystemActions.Player.Jump.performed += Jump;
            _inputSystemActions.Player.Enable();
            IsFacingRight = _spriteRenderer.flipX;

            // SMの初期化
            _stateMachine.Initialize(playerStateMachine.Idle, this);
        }

        private void SetGravity()
        {
            Rigidbody.gravityScale = _baseGravityScale;
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
}