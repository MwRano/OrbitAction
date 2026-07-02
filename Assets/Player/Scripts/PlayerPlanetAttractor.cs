using Orbit.Planet;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Orbit.Player
{
    public class PlayerPlanetAttractor : IStartable
    {
        private const float MinDistanceSqr = 0.01f;

        private readonly PlanetCore _planet;
        private readonly PlanetParams _planetParams;
        private readonly PlanetStateMachine _planetStateMachine;
        private readonly PlayerCore _player;
        private readonly PlayerParam _playerParam;
        private bool _isAttracting;

        [Inject]
        public PlayerPlanetAttractor(
            PlayerCore player,
            PlayerInput playerInput,
            PlayerParam playerParam,
            PlanetCore planet,
            PlanetParams planetParams,
            PlanetStateMachine planetStateMachine)
        {
            _player = player;
            _playerParam = playerParam;
            _planet = planet;
            _planetParams = planetParams;
            _planetStateMachine = planetStateMachine;

            playerInput.Attract
                .Where(isAttract => isAttract && CanAttract())
                .Subscribe(_ => StartAttract())
                .AddTo(_player);

            Observable.EveryUpdate(UnityFrameProvider.FixedUpdate)
                .Where(_ => _isAttracting)
                .Subscribe(_ => Attract())
                .AddTo(_player);
        }

        public void Start()
        {
            // Do nothing
        }

        private bool CanAttract()
        {
            return _planetStateMachine.CurrentState == _planetStateMachine.Deploy;
        }

        private void StartAttract()
        {
            var direction = (Vector2)_planet.transform.position - _player.Rb.position;
            var distance = direction.magnitude;
            if (!CanAttract() ||
                IsInOrbitalRange(distance) ||
                direction.sqrMagnitude <= MinDistanceSqr)
            {
                return;
            }

            _isAttracting = true;
            _player.Rb.AddForce(
                direction.normalized * CalculateInitialImpulse(distance),
                ForceMode2D.Impulse);
            ClampVelocity();
        }

        private void Attract()
        {
            var direction = (Vector2)_planet.transform.position - _player.Rb.position;
            var distance = direction.magnitude;
            if (!CanAttract() ||
                IsInOrbitalRange(distance) ||
                direction.sqrMagnitude <= MinDistanceSqr)
            {
                _isAttracting = false;
                return;
            }

            _player.Rb.AddForce(
                direction.normalized * CalculateAttractForce(distance),
                ForceMode2D.Force);

            ClampVelocity();
        }

        private void ClampVelocity()
        {
            if (_player.Rb.linearVelocity.sqrMagnitude <=
                _playerParam.PlanetAttractMaxSpeed * _playerParam.PlanetAttractMaxSpeed)
            {
                return;
            }

            _player.Rb.linearVelocity =
                _player.Rb.linearVelocity.normalized * _playerParam.PlanetAttractMaxSpeed;
        }

        private bool IsInOrbitalRange(float distance)
        {
            return distance <= _planetParams.OrbitalRange;
        }

        private float CalculateInitialImpulse(float distance)
        {
            return _playerParam.PlanetAttractInitialImpulse * CalculateDistanceRate(distance);
        }

        private float CalculateAttractForce(float distance)
        {
            var rate = CalculateDistanceRate(distance);
            return Mathf.Lerp(
                _playerParam.PlanetAttractForce * 0.25f,
                _playerParam.PlanetAttractForce,
                rate);
        }

        private float CalculateDistanceRate(float distance)
        {
            var orbitalRange = Mathf.Max(_planetParams.OrbitalRange, 0.01f);
            var excessDistance = Mathf.Max(0f, distance - orbitalRange);
            return Mathf.Clamp01(excessDistance / orbitalRange);
        }
    }
}
