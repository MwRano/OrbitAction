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
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera camera = null!;
        [SerializeField] private ParticleSystem moveDust = null!;
        [SerializeField] private ParticleSystem landDust = null!;

        private float _baseGravityScale;
        private bool _canControl = true;
        private InputSystemActions _inputSystemActions = null!;
        private PlayerParam _playerParams = null!;
        private Vector2 _respawnPosition;
        private SpriteRenderer _spriteRenderer = null!;
        private PlayerStateMachine _stateMachine = null!;

        public bool IsGoalReached { get; private set; }
        public bool IsDead { get; set; }
        public Transform PlayerTransform { get; private set; } = null!;
        public Rigidbody2D Rigidbody { get; private set; } = null!;
        public Animator PlayerAnimator { get; private set; } = null!;
        public bool IsGrounded { get; private set; }
        public bool IsFacingRight { get; private set; }
        public Vector2 LookingDirection { get; private set; }

        private void Awake()
        {
            // 着地時の砂埃
            Observable.EveryValueChanged(this, _ => IsGrounded)
                .Where(isGrounded => isGrounded)
                .Subscribe(_ => CreateDust(landDust))
                .AddTo(this);
        }

        private void Update()
        {
            CheckGrounded();
            _stateMachine.Update(this);
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void OnDrawGizmos()
        {
            // _playerParams が未設定ならデフォルト値を使用
            Vector2 offset = _playerParams != null ? _playerParams.GroundCheckOffset : Vector2.down * 0.5f;
            float radius = _playerParams != null ? _playerParams.GroundCheckRadius : 0.1f;

            Vector3 checkPos = (Vector2)transform.position + offset;

            // 実行中は IsGrounded に応じて色を変える。エディタ表示は黄色。
            Gizmos.color = Application.isPlaying ? (IsGrounded ? Color.green : Color.red) : Color.yellow;
            Gizmos.DrawWireSphere(checkPos, radius);

            // プレイヤー位置から判定点への線を描く
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, checkPos);
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

        /// <summary>
        /// playerの操作が可能かどうかを設定するメソッド
        /// </summary>
        /// <param name="value"></param>
        public void SetCanControl(bool value)
        {
            _canControl = value;
        }

        /// <summary>
        /// 物理演算を有効/無効にするメソッド
        /// </summary>
        public void SetIsSimulated(bool isSimulated)
        {
            Rigidbody.simulated = isSimulated;
        }

        public void HandleBuried()
        {
            var isBuried = Physics2D.OverlapCircle(
                transform.position,
                _playerParams.GroundCheckRadius,
                _playerParams.GroundLayer);
            if (!isBuried) return;

            IsDead = true;
            Respawn();
        }

        public void AddImpulse(Vector2 toPos)
        {
            SetIsSimulated(true);
            Rigidbody.linearVelocity = Vector2.zero;
            Rigidbody.AddForce(toPos, ForceMode2D.Impulse);
        }

        public event Action OnRespawn = delegate { };

        private void Respawn()
        {
            Rigidbody.simulated = false;
            SetCanControl(false);
            Observable.Timer(TimeSpan.FromSeconds(0.6f)).Subscribe(_ =>
            {
                transform.position = _respawnPosition;
                Rigidbody.simulated = true;
                SetCanControl(true);
                OnRespawn?.Invoke();
            });
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
            _inputSystemActions.Player.Look.performed += Look;
            _inputSystemActions.Player.Aim.performed += Aim;
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
            if (!_canControl) return;

            Vector2 moveInput = _inputSystemActions.Player.Move.ReadValue<Vector2>();
            if (moveInput.sqrMagnitude <= 0 && Rigidbody.linearVelocity.sqrMagnitude > 0)
                return; // 外部の力で動いている場合には無入力を受け付けない

            Rigidbody.linearVelocity = new Vector2(moveInput.x * _playerParams.MoveSpeed, Rigidbody.linearVelocity.y);

            // 向きに応じてviewの反転    
            _spriteRenderer.flipX = moveInput.x < 0 || !(moveInput.x > 0) && _spriteRenderer.flipX;
            if (IsFacingRight == _spriteRenderer.flipX && IsGrounded) // 向きが変わったときに砂埃を発生
            {
                CreateDust(moveDust);
            }

            IsFacingRight = !_spriteRenderer.flipX;
        }

        private void Look(InputAction.CallbackContext context)
        {
            Vector2 lookInput = context.ReadValue<Vector2>();
            if (lookInput.sqrMagnitude <= 0.1f) return;
            LookingDirection = lookInput.normalized;
        }

        private void Aim(InputAction.CallbackContext context)
        {
            Vector2 aimInput = context.ReadValue<Vector2>();
            float z = camera.WorldToScreenPoint(transform.position).z;
            Vector3 mouseScreen = new Vector3(aimInput.x, aimInput.y, z);
            Vector3 mouseWorld = camera.ScreenToWorldPoint(mouseScreen);

            Vector2 dir = mouseWorld - transform.position;
            if (dir.sqrMagnitude > 0.01f) LookingDirection = dir.normalized;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (!IsGrounded || context.canceled) return;

            Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocity.x, 0);
            Rigidbody.AddForce(Vector2.up * _playerParams.JumpForce, ForceMode2D.Impulse);
            CreateDust(moveDust);
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

        private void CreateDust(ParticleSystem dust)
        {
            dust.Play();
        }
    }
}