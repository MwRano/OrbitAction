#nullable enable
using UnityEngine;
using VContainer;
using UnityEngine.InputSystem;

/// <summary>
/// Planetの挙動を制御するクラス
/// </summary>
public class PlanetController : MonoBehaviour, IPlanetContext
{
    public Transform PlanetTransform { get; private set; } = null!;
    public bool IsLaunched { get; private set; }
    private InputSystemActions _inputSystemActions = null!;
    private PlanetStateMachine _stateMachine = null!;
    
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
        _inputSystemActions.Player.Launch.performed += OnLaunch;
        _inputSystemActions.Player.Enable();
        
        PlanetTransform = transform;
        IsLaunched = false;
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
}
