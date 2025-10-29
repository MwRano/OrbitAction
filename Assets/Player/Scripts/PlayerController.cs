#nullable enable
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using LitMotion;
using LitMotion.Extensions;
using Cysharp.Threading.Tasks;
using R3;

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
        private Vector2 _respawnPosition;
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
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("DeadZone"))
            {
                IsDead = true;
                Respawn();
            }

            if (other.gameObject.CompareTag("Goal")) IsGoalReached = true;
        }

        public bool IsGoalReached { get; private set; }
        public bool IsDead { get; set; }
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
            Rigidbody.linearVelocity = Vector2.zero;
            _canControl = value;
        }

        /// <summary>
        /// 物理演算を有効/無効にするメソッド
        /// </summary>
        public void SetIsSimulated(bool isSimulated)
        {
            Rigidbody.simulated = isSimulated;
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


        // リスポーン位置を設定するメソッド
        public void SetRespawnPosition(Vector2 position)
        {
            _respawnPosition = position;
        }

        private void Respawn()
        {
            Rigidbody.simulated = false;
            SetCanControl(false);
            Observable.Timer(TimeSpan.FromSeconds(0.6f)).Subscribe(_ =>
            {
                transform.position = _respawnPosition;
                Rigidbody.simulated = true;
                SetCanControl(true);
            });
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

        // 公転してから発射するメソッド(クリア時の演出のみ使用)
        public async UniTask StartClearMotionAsync(Transform center, float radius)
        {
            var token = this.GetCancellationTokenOnDestroy();

            // planetが上昇するまで待機
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);


            // 少し上昇するモーション
            await LMotion.Create((Vector2)transform.position, (Vector2)transform.position + Vector2.up, 2.0f)
                .WithEase(Ease.OutSine)
                .BindToPositionXY(transform)
                .ToUniTask(cancellationToken: token);


            // XY公転モーション
            Vector3 toCenter = transform.position - center.position;
            float startAngle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;
            float totalAngle = 360f * 10 + 90f;
            Vector2 force = Vector2.up * 100f;
            await LMotion.Create(startAngle, startAngle + totalAngle, 10)
                .WithEase(Ease.InCubic) // 徐々に加速するイージング
                .WithOnComplete(() =>
                {
                    Rigidbody.gravityScale = 0;
                    Rigidbody.AddForce(force, ForceMode2D.Impulse);
                })
                .Bind(angle =>
                {
                    float rad = angle * Mathf.Deg2Rad;
                    Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * (radius - 1.0f);
                    transform.position = center.position + offset;
                })
                .ToUniTask(cancellationToken: token);
        }
    }
}