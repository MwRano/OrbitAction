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
        private readonly DeployPositionCalculator _deployPositionCalculator;
        private readonly PlanetParams _planetParams;
        private readonly PlayerController _player;
        private bool _isReached;

        [Inject]
        public TravelState(
            PlayerController player,
            PlanetParams planetParams,
            DeployPositionCalculator deployPositionCalculator)
        {
            _player = player;
            _planetParams = planetParams;
            _deployPositionCalculator = deployPositionCalculator;
        }

        public void Enter(IPlanetContext planet)
        {
            // 移動モーション 
            float planetRadius = planet.PlanetSpriteRenderer.bounds.extents.x;
            Vector2 destPos = _deployPositionCalculator.Calculate(
                _player.PlayerTransform.position,
                _player.LookingDirection,
                planet.PlanetTransform.position,
                planetRadius
            );

            if (_player.IsGoalReached)
            {
                destPos = (Vector2.up * _planetParams.OrbitalRange) + (Vector2)_player.PlayerTransform.position;
            }

            // deply地点にlaunch
            LMotion.Create((Vector2)planet.PlanetTransform.position, destPos, 0.3f)
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
}