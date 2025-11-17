using R3;
using UnityEngine;
using System;

namespace Player
{
    public class PlayerAimer
    {
        private readonly PlayerCore _player;

        public PlayerAimer(PlayerInput playerInput, PlayerCore player)
        {
            _player = player;

            playerInput.Look
                .Where(l => l.sqrMagnitude >= 0.1f * 0.1f)
                .Subscribe(Look);

            playerInput.Aim
                .Subscribe(Aim);
        }

        public Vector2 AimDirection { get; private set; }

        private void Look(Vector2 lookInput)
        {
            AimDirection = lookInput.normalized;
        }

        private void Aim(Vector2 aimInput)
        {
            if (Camera.main == null) return;

            Vector2 mouseScreen = new Vector3(aimInput.x, aimInput.y);
            var mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

            Vector2 dir = mouseWorld - _player.Rb.transform.position;
            if (dir.sqrMagnitude > 0.01f) AimDirection = dir.normalized;
        }
    }
}