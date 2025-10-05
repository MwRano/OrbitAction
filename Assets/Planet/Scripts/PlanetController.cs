#nullable enable
using UnityEngine;
using VContainer;
using LitMotion;
using LitMotion.Extensions;

/// <summary>
/// Planetの挙動を制御するクラス
/// </summary>
public class PlanetController : MonoBehaviour
{
    private PlayerController _player = null!;
    private float _smoothTime;
    private float _maxSpeed;
    private Vector2 _currentVelocity;
    
    private MotionHandle _floatingMotion;
    private bool _isFloating = false;
    
    [Inject]
    public void Construct(PlayerController playerController, PlanetParams planetParams)
    {
        _player = playerController;
        _smoothTime = planetParams.SmoothTime;
        _maxSpeed = planetParams.MaxSpeed;
    }

    private void Awake()
    {
        StartRotationMotion();
    }
    
    private void Update()
    {
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
        bool shouldFloat = _player.Rigidbody.linearVelocity.magnitude < 0.2f && _currentVelocity.magnitude < 0.1f;
        if (!_isFloating && shouldFloat) 
        {
            _floatingMotion = StartFloatingMotion();
            _isFloating = true;
        }
        else if(_isFloating && !shouldFloat)
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
        Vector2 playerPos = _player.transform.position;
        Vector2 targetPosition = _player.IsFacingRight
            ? new Vector2(playerPos.x - 1, playerPos.y + 1)
            : new Vector2(playerPos.x + 1, playerPos.y + 1);
        
        if(!_isFloating) FollowTarget(targetPosition);
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
}
