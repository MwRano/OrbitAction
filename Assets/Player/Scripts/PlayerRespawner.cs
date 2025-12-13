using System;
using Orbit.Planet;
using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;

namespace Orbit.Player
{
    public class PlayerRespawner
    {
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

            _player.Collider.OnTriggerEnter2DAsObservable()
                .Where(other => other.CompareTag("RespawnPoint"))
                .Subscribe(other => SetRespawnPosition(other.transform.position));
        }

        public ReadOnlyReactiveProperty<bool> IsRespawning => _isRespawning;

        private void SetRespawnPosition(Vector2 respawnPosition)
        {
            _respawnPosition = respawnPosition;
        }

        private void Respawn()
        {
            _player.Rb.simulated = false;

            // Death animationの待機
            Observable.Timer(TimeSpan.FromSeconds(0.62f))
                .Subscribe(_ =>
                {
                    _isRespawning.Value = true;
                    _player.Sprite.enabled = false;
                })
                .AddTo(_player);

            Observable.Timer(TimeSpan.FromSeconds(1.2f))
                .Subscribe(_ =>
                {
                    _planetInput.ResetLaunch();
                    _isRespawning.Value = false;
                    _player.transform.position = _respawnPosition;
                    _player.Sprite.enabled = true;
                    _player.Rb.linearVelocity = Vector2.zero;
                    _player.Rb.simulated = true;

                    _planet.transform.position = _respawnPosition + Vector2.one;
                })
                .AddTo(_player);
        }
    }
}