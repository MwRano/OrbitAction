#nullable enable
using UnityEngine;
using VContainer;
using UnityEngine.InputSystem;

/// <summary>
/// Planetの挙動を制御するクラス
/// </summary>
public class PlanetController : MonoBehaviour, IPlanetContext
{
    private InputSystemActions _inputSystemActions = null!;
    private PlanetStateMachine _stateMachine = null!;

    private void Awake()
    {
        // InputSystemへのメソッド登録
        _inputSystemActions.Planet.Launch.performed += OnLaunch;
        _inputSystemActions.Planet.Enable();

        PlanetTransform = transform;
        IsLaunched = false;
        _stateMachine.Initialize(_stateMachine.Follow, this);
    }

    private void Update()
    {
        _stateMachine.Update(this);
    }

    public Transform PlanetTransform { get; private set; } = null!;
    public bool IsLaunched { get; private set; }

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
}