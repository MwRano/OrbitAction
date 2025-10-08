#nullable enable
using UnityEngine;
using VContainer;
using UnityEngine.InputSystem;

/// <summary>
/// Planetの挙動を制御するクラス
/// </summary>
public class PlanetController : MonoBehaviour, IPlanetContext
{
    [SerializeField] private GameObject attractionAreaView = null!;
    [SerializeField] private GameObject orbitAreaView = null!;

    private InputSystemActions _inputSystemActions = null!;
    private PlanetStateMachine _stateMachine = null!;

    private void Awake()
    {
        // InputSystemへのメソッド登録
        _inputSystemActions.Planet.Launch.performed += OnLaunch;
        _inputSystemActions.Planet.Attract.performed += OnAttract;
        _inputSystemActions.Planet.Enable();

        PlanetTransform = transform;
        IsLaunched = false;
        AttractionAreaSpriteRenderer = attractionAreaView.GetComponent<SpriteRenderer>();
        OrbitAreaSpriteRenderer = orbitAreaView.GetComponent<SpriteRenderer>();
        _stateMachine.Initialize(_stateMachine.Follow, this);
    }

    private void Update()
    {
        _stateMachine.Update(this);
    }

    public Transform PlanetTransform { get; private set; } = null!;
    public bool IsLaunched { get; private set; }
    public SpriteRenderer AttractionAreaSpriteRenderer { get; private set; } = null!;
    public SpriteRenderer OrbitAreaSpriteRenderer { get; private set; } = null!;
    public GameObject AttractionAreaView => attractionAreaView;
    public GameObject OrbitAreaView => orbitAreaView;


    [Inject]
    public void Construct(
        InputSystemActions inputSystemActions,
        PlanetStateMachine stateMachine)
    {
        _inputSystemActions = inputSystemActions;
        _stateMachine = stateMachine;
    }

    private void OnLaunch(InputAction.CallbackContext context)
    {
        IsLaunched ^= true;
    }

    private void OnAttract(InputAction.CallbackContext context)
    {
        // DeployStateのときのみAttractを実行
        if (_stateMachine.CurrentState is DeployState deployState)
        {
            deployState.Attract(PlanetTransform.position);
        }
    }
}