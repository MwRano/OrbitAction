#nullable enable
using LitMotion;
using LitMotion.Extensions;
using Player;
using UnityEngine;
using VContainer;

namespace Planet
{
    public class TravelState : IPlanetState
    {
        private readonly float _launchDistance;
        private readonly IPlayerContext _player;
        private bool _isReached;

        [Inject]
        public TravelState(IPlayerContext player, PlanetParams planetParams)
        {
            _player = player;
            _launchDistance = planetParams.LaunchDistance;
        }

        public void Enter(IPlanetContext planet)
        {
            // 移動モーション 
            // Vector2 destPos = _launchDistance * _player.LookingDirection + (Vector2)planet.PlanetTransform.position;
            Vector2 destPos = _launchDistance * _player.LookingDirection + (Vector2)_player.PlayerTransform.position;
            LMotion.Create((Vector2)planet.PlanetTransform.position, destPos, 0.3f)
                .WithEase(Ease.OutCubic)
                .WithOnComplete(() => _isReached = true)
                .BindToPositionXY(planet.PlanetTransform)
                .AddTo(planet.PlanetTransform);

            _player.DisableGravity();
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
}