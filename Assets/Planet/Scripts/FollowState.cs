#nullable enable
using UnityEngine;
using VContainer;
using LitMotion;
using LitMotion.Extensions;

public class FollowState :IPlanetState
{
    private readonly IPlayerContext _player;
    private readonly float _smoothTime;
    private readonly float _maxSpeed;
    private MotionHandle _rotationMotion;
    private Vector2 _currentVelocity;
    
    [Inject]
    public FollowState(IPlayerContext player, PlanetParams planetParams)
    {
        _player = player;
        _smoothTime = planetParams.SmoothTime;
        _maxSpeed = planetParams.MaxSpeed;
    }
    
    public void Enter(IPlanetContext planet)
    {
        if(_rotationMotion.IsActive()) return;
        
        _rotationMotion = LMotion.Create(0f, 360f, 30f)
            .WithEase(Ease.Linear)
            .WithLoops(-1)
            .BindToLocalEulerAnglesZ(planet.PlanetTransform)
            .AddTo(planet.PlanetTransform);
    }

    public void Update(IPlanetContext planet, PlanetStateMachine stateMachine)
    {
        Vector2 playerPos = _player.PlayerTransform.position;
        Vector2 targetPosition = _player.IsFacingRight　// playerの向きに応じて追従位置を変更
            ? new Vector2(playerPos.x - 1, playerPos.y + 1)
            : new Vector2(playerPos.x + 1, playerPos.y + 1);
        
        planet.PlanetTransform.position = Vector2.SmoothDamp(
            planet.PlanetTransform.position,
            targetPosition,
            ref _currentVelocity,
            _smoothTime,
            _maxSpeed
        );
        
        // 状態遷移の判定
        if(_player.Rigidbody.linearVelocity.sqrMagnitude < 0.01f && _currentVelocity.sqrMagnitude < 0.01f) // playerが停止したらIdleへ遷移
        {
            stateMachine.TransitionTo(stateMachine.Hover, planet);
        }
        else if(planet.IsLaunched)
        {
            stateMachine.TransitionTo(stateMachine.Travel, planet);
        }
    }

    public void Exit()
    {
        
    }
}
