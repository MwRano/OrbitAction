using Orbit.Planet;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Orbit.Player
{
    public class PlayerOrbitAssist : IStartable
    {
        private readonly float _defaultGravityScale;
        private readonly PlanetCore _planet;
        private readonly PlanetParams _planetParams;
        private readonly PlanetStateMachine _planetStateMachine;
        private readonly PlayerCore _player;
        private readonly PlayerParam _playerParam;

        [Inject]
        public PlayerOrbitAssist(
            PlayerCore player,
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
            _defaultGravityScale = player.Rb.gravityScale;

            Observable.EveryUpdate()
                .Subscribe(_ => UpdateGravityScale())
                .AddTo(_player);

            Observable.EveryUpdate(UnityFrameProvider.FixedUpdate)
                .Subscribe(_ => ApplyInertiaDamping())
                .AddTo(_player);
        }

        public void Start()
        {
            // Do nothing
        }

        private void UpdateGravityScale()
        {
            _player.Rb.gravityScale = IsOrbitAssistActive()
                ? _defaultGravityScale * _playerParam.PlanetOrbitGravityScaleRate
                : _defaultGravityScale;
        }

        private void ApplyInertiaDamping()
        {
            if (!IsOrbitAssistActive() || _player.IsGrounded.CurrentValue) return;

            var dampingRate = 1f - Mathf.Exp(-_playerParam.PlanetOrbitInertiaDamping * Time.fixedDeltaTime);
            _player.Rb.linearVelocity = Vector2.Lerp(_player.Rb.linearVelocity, Vector2.zero, dampingRate);
        }

        private bool IsOrbitAssistActive()
        {
            return _planetStateMachine.CurrentState == _planetStateMachine.Deploy &&
                   IsPlayerInOrbitalRange();
        }

        private bool IsPlayerInOrbitalRange()
        {
            var distanceSqr = ((Vector2)_planet.transform.position - _player.Rb.position).sqrMagnitude;
            return distanceSqr <= _planetParams.OrbitalRange * _planetParams.OrbitalRange;
        }
    }
}
