using System;
using R3;
using UnityEngine;
using VContainer;

namespace Orbit.Player
{
    public class PlayerRespawner
    {
        private Vector2 _respawnPosition;
        private readonly PlayerCore _player;
        
        public Action OnRespawn { get; set; }
        
        [Inject]
        public PlayerRespawner(PlayerCore player)
        {
            _player = player;
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
            Observable.Timer(TimeSpan.FromSeconds(0.635f))
                .Subscribe(_ =>
                {
                    OnRespawn?.Invoke();
                    _player.Rb.transform.position = _respawnPosition;
                    _player.Rb.linearVelocity = Vector2.zero;
                    _player.Rb.simulated = true;
                })
                .AddTo(_player);
        }
    }
}