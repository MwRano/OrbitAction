#nullable enable
using UnityEngine;
using VContainer;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.InputSystem;

/// <summary>
/// Planetの挙動を制御するクラス
/// </summary>
public class PlanetController : MonoBehaviour
{
    private InputSystemActions _inputSystemActions = null!;
    private PlayerController _player = null!;
    
    // 追従モーションのパラメータ
    private float _smoothTime;
    private float _maxSpeed;
    private Vector2 _currentVelocity;
    
    // launchのパラメータ
    private float _launchDistance;
    
    // Motionのフラグ
    private MotionHandle _floatingMotion;
    private bool _isFloating;
    private bool _isOwnedByPlayer;
    
    [Inject]
    public void Construct(
        PlayerController playerController, 
        PlanetParams planetParams,
        InputSystemActions inputSystemActions)
    {
        _player = playerController;
        _smoothTime = planetParams.SmoothTime;
        _maxSpeed = planetParams.MaxSpeed;
        _launchDistance = planetParams.LaunchDistance;
        _inputSystemActions = inputSystemActions;
    }

    private void Awake()
    {
        // InputSystemへのメソッド登録
        _inputSystemActions.Player.Launch.performed += Launch;
        _inputSystemActions.Player.Enable();
        
        // フラグの初期化
        _isOwnedByPlayer = true;
        _isFloating = false;
        StartRotationMotion();
    }
    
    private void Update()
    {
        // Motionの制御
        ControlFloatingMotion();
        ControlFollowingMotion();
    }

    private MotionHandle StartRotationMotion()
    {
        return LMotion.Create(0f, 360f, 30f)
            .WithEase(Ease.Linear)
            .WithLoops(-1, LoopType.Restart)
            .BindToLocalEulerAnglesZ(transform)
            .AddTo(transform);
    }
    
    /// <summary>
    /// 浮遊モーションの制御
    /// </summary>
    private void ControlFloatingMotion()
    {
        bool shouldFloat = _player.Rigidbody.linearVelocity.sqrMagnitude < 0.2f && _currentVelocity.sqrMagnitude < 0.1f || !_isOwnedByPlayer;
        
        if (!_isFloating && shouldFloat) // プレイヤーが低速な場合に浮遊モーションを開始
        {
            _floatingMotion = StartFloatingMotion();
            _isFloating = true;
        }
        else if(_isFloating && !shouldFloat)　// プレイヤーが高速な場合に浮遊モーションを停止
        {
            _floatingMotion.Cancel();
            _isFloating = false;
        }
    }

    private MotionHandle StartFloatingMotion()
    {
        return LMotion.Create(transform.position.y, transform.position.y - 0.2f, 1f)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToPositionY(transform)
                .AddTo(transform);
    }
    
    /// <summary>
    /// 追従モーションの制御
    /// </summary>
    private void ControlFollowingMotion()
    {
        // Playerから少し離れた位置に追従
        Vector2 playerPos = _player.transform.position;
        Vector2 targetPosition = _player.IsFacingRight　// playerの向きに応じて追従位置を変更
            ? new Vector2(playerPos.x - 1, playerPos.y + 1)
            : new Vector2(playerPos.x + 1, playerPos.y + 1);
        
        if(!_isFloating && _isOwnedByPlayer) FollowTarget(targetPosition);
    }
    
    private void FollowTarget(Vector2 targetPosition)
    {
        transform.position = Vector2.SmoothDamp(
            transform.position,
            targetPosition,
            ref _currentVelocity,
            _smoothTime,
            _maxSpeed
        );
    }
    
    /// <summary>
    /// 指定方向に惑星を飛ばすメソッド
    /// </summary>
    /// <param name="context"></param>
    private void Launch(InputAction.CallbackContext context)
    {
        if (!_isOwnedByPlayer) return;
        
        _isOwnedByPlayer = false;
        if(_floatingMotion.IsActive())_floatingMotion.Cancel();
        
        Vector2 destPos = _launchDistance * _player.LookingDirection + (Vector2)transform.position;
        LMotion.Create((Vector2)transform.position, destPos, 0.5f)
            .WithEase(Ease.OutCubic)
            .WithOnComplete(()=> _isFloating = false)
            .BindToPositionXY(transform)
            .AddTo(transform);
    }
}
