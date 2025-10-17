#nullable enable
using UnityEngine;
using VContainer;
using UnityEngine.InputSystem;

/// <summary>
/// Planetの挙動を制御するクラス
/// </summary>
public class PlanetController : MonoBehaviour, IPlanetContext
{
    [SerializeField] private GameObject orbitAreaView = null!;

    private InputSystemActions _inputSystemActions = null!;
    private PlanetStateMachine _stateMachine = null!;
    
    public Transform PlanetTransform { get; private set; } = null!;
    public bool IsLaunched { get; private set; }
    public SpriteRenderer OrbitAreaSpriteRenderer { get; private set; } = null!;
    public GameObject OrbitAreaView => orbitAreaView;

    [Inject]
    public void Construct(
        InputSystemActions inputSystemActions,
        PlanetStateMachine stateMachine)
    {
        _inputSystemActions = inputSystemActions;
        _stateMachine = stateMachine;
    }
    
    private void Awake()
    {
        // InputSystemへのメソッド登録
        _inputSystemActions.Planet.Launch.performed += OnLaunch;
        _inputSystemActions.Planet.Attract.performed += OnOrbit;
        _inputSystemActions.Planet.Enable();

        PlanetTransform = transform;
        IsLaunched = false;
        OrbitAreaSpriteRenderer = orbitAreaView.GetComponent<SpriteRenderer>();
        _stateMachine.Initialize(_stateMachine.Follow, this);
    }

    private void Update()
    {
        _stateMachine.Update(this);
    }
    
    private void OnLaunch(InputAction.CallbackContext context)
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
}