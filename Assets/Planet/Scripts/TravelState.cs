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
        private readonly PlanetController _planet;
        private bool _isReached;

        [Inject]
        public TravelState(
            PlayerCore player,
            PlayerAimer playerAimer,
            PlanetParams planetParams,
            DeployPositionCalculator deployPositionCalculator,
            PlanetController planet)
        {
            _player = player;
            _playerAimer = playerAimer;
            _planetParams = planetParams;
            _deployPositionCalculator = deployPositionCalculator;
            _planet = planet;
        }

        public void Enter()
        {
            // 移動モーション 
            float planetRadius = _planet.PlanetSpriteRenderer.bounds.extents.x;
            Vector2 destPos = _deployPositionCalculator.Calculate(
                _player.Rb.transform.position,
                _playerAimer.AimDirection,
                _planet.PlanetTransform.position,
                planetRadius
            );

            if (_player.IsGoalReached.CurrentValue)
            {
                destPos = (Vector2.up * _planetParams.OrbitalRange) + (Vector2)_player.Rb.transform.position;
            }

            // deply地点にlaunch
            LMotion.Create((Vector2)_planet.PlanetTransform.position, destPos, 0.3f)
                .WithEase(Ease.OutCubic)
                .WithOnComplete(() => _isReached = true)
                .BindToPositionXY(_planet.PlanetTransform)
                .AddTo(_planet.PlanetTransform);
        }

        public void Update(PlanetStateMachine stateMachine)
        {
            // 到達すればdeployに遷移
            if (_isReached) stateMachine.TransitionTo(stateMachine.Deploy);
        }

        public void Exit()
        {
            _isReached = false;
        }
    }
}