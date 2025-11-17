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
        private readonly PlayerCore _player;
        private readonly PlayerAimer _playerAimer;
        private bool _isReached;

        [Inject]
        public TravelState(
            PlayerCore player,
            PlayerAimer playerAimer,
            PlanetParams planetParams,
            DeployPositionCalculator deployPositionCalculator)
        {
            _player = player;
            _playerAimer = playerAimer;
            _planetParams = planetParams;
            _deployPositionCalculator = deployPositionCalculator;
        }

        public void Enter(PlanetController planet)
        {
            // 移動モーション 
            float planetRadius = planet.PlanetSpriteRenderer.bounds.extents.x;
            Vector2 destPos = _deployPositionCalculator.Calculate(
                _player.Rb.transform.position,
                _playerAimer.AimDirection,
                planet.PlanetTransform.position,
                planetRadius
            );

            if (_player.IsGoalReached.Value)
            {
                destPos = (Vector2.up * _planetParams.OrbitalRange) + (Vector2)_player.Rb.transform.position;
            }

            // deply地点にlaunch
            LMotion.Create((Vector2)planet.PlanetTransform.position, destPos, 0.3f)
                .WithEase(Ease.OutCubic)
                .WithOnComplete(() => _isReached = true)
                .BindToPositionXY(planet.PlanetTransform)
                .AddTo(planet.PlanetTransform);
        }

        public void Update(PlanetController planet, PlanetStateMachine stateMachine)
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