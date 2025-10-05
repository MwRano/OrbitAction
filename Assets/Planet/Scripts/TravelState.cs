#nullable enable
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using VContainer;

public class TravelState : IPlanetState
{
    private IPlayerContext _player;
    float _launchDistance;
    
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
            .BindToPositionXY(planet.PlanetTransform)
            .AddTo(planet.PlanetTransform);
    }

    public void Update(IPlanetContext planet, PlanetStateMachine stateMachine)
    {
        
    }

    public void Exit()
    {
        
    }
}
