#nullable enable
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using VContainer;

public class TravelState : IPlanetState
{
    private readonly IPlayerContext _player;
    private readonly float _launchDistance;
    private bool _isReached;
    
    [Inject]
    public TravelState(IPlayerContext player, PlanetParams planetParams)
    {
        _player = player;
        _launchDistance = planetParams.LaunchDistance;
    }
    public void Enter(IPlanetContext planet)
    {
        Vector2 destPos = _launchDistance * _player.LookingDirection + (Vector2)planet.PlanetTransform.position;
        LMotion.Create((Vector2)planet.PlanetTransform.position, destPos, 0.5f)
            .WithEase(Ease.OutCubic)
            .WithOnComplete(() => _isReached = true)
            .BindToPositionXY(planet.PlanetTransform)
            .AddTo(planet.PlanetTransform);
    }

    public void Update(IPlanetContext planet, PlanetStateMachine stateMachine)
    {
        // 到達すればdeployに遷移
        if (_isReached) stateMachine.TransitionTo(stateMachine.Deploy, planet);
    }

    public void Exit()
    {
        _isReached = false;
    }
}
