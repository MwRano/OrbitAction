using System;
using Orbit.Planet;
using R3;
using UnityEngine;
using VContainer;

namespace Orbit.Player
{
    public class PlayerRespawner
    {
        public ReadOnlyReactiveProperty<bool> IsRespawning => _isRespawning;
        
        private readonly ReactiveProperty<bool> _isRespawning = new();
        private readonly PlanetCore _planet;
        private readonly PlanetInput _planetInput;
        private readonly PlayerCore _player;
        private Vector2 _respawnPosition;

        [Inject]
        public PlayerRespawner(PlayerCore player, PlanetCore planet, PlanetInput planetInput)
        {
            _player = player;
            _planet = planet;
            _planetInput = planetInput;
            player.IsDead
                .Where(isDead => isDead)
                .Subscribe(_ => Respawn())
                .AddTo(player);
        }

        public void SetRespawnPosition(Vector2 respawnPosition)
        {
            _respawnPosition = respawnPosition;
        }

        private void Respawn()
        {
            _player.Rb.simulated = false;
            _isRespawning.Value = true;

            // Deply状態のfloat motionの解除待つため、少し早くリセット
            Observable.Timer(TimeSpan.FromSeconds(0.6f))
                .Subscribe(_ => _planetInput.ResetLaunch())
                .AddTo(_player);

            Observable.Timer(TimeSpan.FromSeconds(0.635f))
                .Subscribe(_ =>
                {
                    _isRespawning.Value = false;
                    _player.transform.position = _respawnPosition;
                    _player.Rb.linearVelocity = Vector2.zero;
                    _player.Rb.simulated = true;

                    _planet.transform.position = _respawnPosition + Vector2.one;
                })
                .AddTo(_player);
        }
    }
}