#nullable enable
using LitMotion;
using LitMotion.Extensions;
using Player;
using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;
using System;

namespace Planet
{
    public class TravelState : IPlanetState
    {
        private readonly PlanetParams _planetParams;
        private readonly IPlayerContext _player;
        private bool _isReached;
        private MotionHandle? _motionHandle;
        private IDisposable? _triggerSubscription;

        [Inject]
        public TravelState(
            IPlayerContext player,
            PlanetParams planetParams)
        {
            _player = player;
            _planetParams = planetParams;
        }

        public void Enter(IPlanetContext planet)
        {
            // 移動モーション 
            Vector2 destPos = _planetParams.LaunchDistance * _player.LookingDirection +
                              (Vector2)_player.PlayerTransform.position;
            if (_player.IsGoalReached)
            {
                destPos = (Vector2.up * _planetParams.OrbitalRange) + (Vector2)_player.PlayerTransform.position;
            }

            _motionHandle = LMotion.Create((Vector2)planet.PlanetTransform.position, destPos, 0.3f)
                .WithEase(Ease.OutCubic)
                .WithOnComplete(() => _isReached = true)
                .BindToPositionXY(planet.PlanetTransform);

            _triggerSubscription = planet.PlanetTransform.OnTriggerEnter2DAsObservable()
                .Where(collision => !collision.gameObject.CompareTag("Player"))
                .Subscribe(_ =>
                {
                    if (_motionHandle.HasValue) _motionHandle.Value.TryCancel();
                    _isReached = true;
                });
        }

        public void Update(IPlanetContext planet, PlanetStateMachine stateMachine)
        {
            // 到達すればdeployに遷移
            if (_isReached) stateMachine.TransitionTo(stateMachine.Deploy, planet);
        }

        public void Exit()
        {
            if (_motionHandle.HasValue)
            {
                _motionHandle.Value.TryCancel();
                _motionHandle = null;
            }

            _triggerSubscription?.Dispose();
            _triggerSubscription = null;

            _isReached = false;
        }
    }
}