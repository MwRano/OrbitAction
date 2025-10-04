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
    private Vector2 _currentVelocity;
    
    private MotionHandle _floatingMotion;
    private bool _isFloating = false;
    
    [Inject]
    public void Construct(PlayerController playerController, PlanetParams planetParams)
    {
        _player = playerController;
    }
    
    private void Update()
    {
        ControlFloatingMotion();
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
}
