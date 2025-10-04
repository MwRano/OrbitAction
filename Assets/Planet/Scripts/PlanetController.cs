#nullable enable
using UnityEngine;
using VContainer;


public class PlanetController : MonoBehaviour
{
    private PlayerController _player = null!;
    private float _smoothTime;
    private float _maxSpeed;
    private Vector2 _currentVelocity;
    
    [Inject]
    public void Construct(PlayerController playerController, PlanetParams planetParams)
    {
        _player = playerController;
        _smoothTime = planetParams.SmoothTime;
        _maxSpeed = planetParams.MaxSpeed;
    }
    private void Update()
    {
        Vector2 playerPos = _player.transform.position;
        Vector2 targetPosition = _player.IsFacingRight
            ? new Vector2(playerPos.x - 1, playerPos.y + 1)
            : new Vector2(playerPos.x + 1, playerPos.y + 1);
        FollowTarget(targetPosition);
    }
    
    /// <summary>
    /// 目標に追従するメソッド
    /// </summary>
    /// <param name="targetPosition"></param>
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
