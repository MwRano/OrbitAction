#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using R3;
using Player;

namespace Planet
{
    /// <summary>
    /// Planetの挙動を制御するクラス
    /// </summary>
    public class PlanetController : MonoBehaviour, IPlanetContext
    {
        [SerializeField] private GameObject orbitAreaView = null!;

        private InputSystemActions _inputSystemActions = null!;
        private PlayerController _player = null!;
        private PlanetStateMachine _stateMachine = null!;

        private void Awake()
        {
            // InputSystemへのメソッド登録
            _inputSystemActions.Planet.Launch.performed += OnLaunch;
            _inputSystemActions.Planet.Orbit.performed += OnOrbit;
            _inputSystemActions.Planet.Enable();

            PlanetTransform = transform;
            IsLaunched = false;
            OrbitAreaSpriteRenderer = orbitAreaView.GetComponent<SpriteRenderer>();
            _stateMachine.Initialize(_stateMachine.Follow, this);

            // playerが死亡したら発射状態を解除
            Observable.EveryValueChanged(_player, p => p.IsDead)
                .Where(isDead => isDead)
                .Subscribe(_ => IsLaunched = false)
                .AddTo(this);
        }

        private void Update()
        {
            _stateMachine.Update(this);
        }

        public Transform PlanetTransform { get; private set; } = null!;
        public SpriteRenderer PlanetSpriteRenderer => GetComponent<SpriteRenderer>();
        public bool IsLaunched { get; private set; }
        public SpriteRenderer OrbitAreaSpriteRenderer { get; private set; } = null!;
        public GameObject OrbitAreaView => orbitAreaView;

        [Inject]
        public void Construct(
            InputSystemActions inputSystemActions,
            PlanetStateMachine stateMachine,
            PlayerController player)
        {
            _inputSystemActions = inputSystemActions;
            _stateMachine = stateMachine;
            _player = player;
            player.OnRespawn += Respawn;
        }

        public void OnLaunch(InputAction.CallbackContext context)
        {
            IsLaunched ^= true;
        }

        private void OnOrbit(InputAction.CallbackContext context)
        {
            // DeployStateのときのみAttractを実行
            if (_stateMachine.CurrentState is DeployState deployState)
            {
                deployState.Orbit(PlanetTransform.position);
            }
        }

        private void Respawn()
        {
            IsLaunched = false;
            transform.position = (Vector2)_player.transform.position + Vector2.up;
        }
    }
}