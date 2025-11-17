using System;
using R3;
using UnityEngine;

namespace Player
{
    public class PlayerRespawner
    {
        private Vector2 _respawnPosition;

        public Action OnRespawn;

        public PlayerRespawner(PlayerCore player)
        {
            player.IsDead
                .Where(isDead => isDead)
                .Subscribe(_ =>
                {
                    player.Rb.simulated = false;
                    Respawn(player.Rb);
                });
        }

        public void SetRespawnPosition(Vector2 respawnPosition)
        {
            _respawnPosition = respawnPosition;
        }

        private void Respawn(Rigidbody2D rb)
        {
            rb.simulated = false;
            Observable.Timer(TimeSpan.FromSeconds(0.635f))
                .Subscribe(_ =>
                {
                    OnRespawn?.Invoke();
                    rb.transform.position = _respawnPosition;
                    rb.linearVelocity = Vector2.zero;
                    rb.simulated = true;
                });
        }
    }
}