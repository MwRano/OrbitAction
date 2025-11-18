#nullable enable
using Orbit.Player;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Orbit.Planet
{
    /// <summary>
    /// Planetの挙動を制御するクラス
    /// </summary>
    public class PlanetController : MonoBehaviour
    {
        [SerializeField] private GameObject orbitAreaView = null!;

        private InputSystemActions _inputSystemActions = null!;
        private PlayerCore _player = null!;

        public Transform PlanetTransform { get; private set; } = null!;
        public SpriteRenderer PlanetSpriteRenderer => GetComponent<SpriteRenderer>();
        public bool IsLaunched { get; private set; }
        public SpriteRenderer OrbitAreaSpriteRenderer { get; private set; } = null!;
        public GameObject OrbitAreaView => orbitAreaView;

        private void Awake()
        {
            // InputSystemへのメソッド登録
            _inputSystemActions.Planet.Launch.performed += OnLaunch;
            // _inputSystemActions.Planet.Orbit.performed += OnOrbit;
            _inputSystemActions.Planet.Enable();
            
            IsLaunched = false;
            OrbitAreaSpriteRenderer = orbitAreaView.GetComponent<SpriteRenderer>();

            _player.IsDead
                .Where(isDead => isDead)
                .Subscribe(_ => IsLaunched = false)
                .AddTo(this);
        }

        [Inject]
        public void Construct(
            InputSystemActions inputSystemActions,
            PlayerCore player,
            PlayerRespawner playerRespawner)
        {
            _inputSystemActions = inputSystemActions;
            _player = player;
            playerRespawner.OnRespawn += Respawn;
            
            PlanetTransform = transform;
        }

        public void OnLaunch(InputAction.CallbackContext context)
        {
            IsLaunched ^= true;
        }

        private void Respawn()
        {
            IsLaunched = false;
            transform.position = (Vector2)_player.transform.position + Vector2.up;
        }
    }
}