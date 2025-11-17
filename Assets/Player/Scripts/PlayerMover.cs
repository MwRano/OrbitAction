using Planet;
using R3;
using UnityEngine;
using System;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using VContainer.Unity;

namespace Player
{
    public class PlayerMover : ITickable
    {
        private readonly PlanetController _planet;
        private readonly PlanetParams _planetParams;
        private readonly PlayerCore _player;
        private readonly PlayerParam _playerParams;
        private readonly CompositeDisposable _disposable = new();
        private bool _isOrbiting;

        public PlayerMover(PlayerParam playerParam,
            PlayerCore playerCore,
            PlayerInput playerInput,
            PlanetStateMachine planetStateMachine,
            PlanetController planetController,
            PlanetParams planetParams)
        {
            _playerParams = playerParam;
            _player = playerCore;
            _planet = planetController;
            _planetParams = planetParams;

            playerInput.Jump
                .Where(isJump => isJump)
                .Subscribe(_ => Jump())
                .AddTo(_player);

            playerInput.Orbit
                .Where(isOrbit => isOrbit &&
                                  planetStateMachine.CurrentState == planetStateMachine.Deploy)
                .Subscribe(_ => Orbit(_planet.PlanetTransform.position))
                .AddTo(_player);

            Observable.EveryUpdate(UnityFrameProvider.FixedUpdate)
                .Where(_ => playerInput.Move.sqrMagnitude > 0 ||
                            _player.Rb.linearVelocity.sqrMagnitude <= 0)
                .Subscribe(_ => Move(playerInput.Move))
                .AddTo(_player);
        }

        public void Tick()
        {
        }

        private void Move(Vector2 moveInput)
        {
            _player.Rb.linearVelocity =
                new Vector2(moveInput.x * _playerParams.MoveSpeed, _player.Rb.linearVelocity.y);

            // 向きに応じてviewの反転
            var preFlipX = _player.Sprite.flipX;
            _player.Sprite.flipX = moveInput.x < 0 || !(moveInput.x > 0) && _player.Sprite.flipX;
            // if (sprite.flipX != preFlipX)
            // {
            //     CreateDust(moveDust);
            // }
        }

        private void Jump()
        {
            if (!_player.IsGrounded.CurrentValue) return;
            _player.Rb.linearVelocity = new Vector2(_player.Rb.linearVelocity.x, 0);
            _player.Rb.AddForce(Vector2.up * _playerParams.JumpForce, ForceMode2D.Impulse);
        }

        private void Orbit(Vector2 planetPosition)
        {
            var colliders = GetCollidersInCircle(planetPosition, _planetParams.OrbitalRange, "Orbitable");
            if (colliders.Length == 0 || _isOrbiting) return;

            _isOrbiting = true;
            foreach (var col in colliders)
            {
                CreateOrbitMotion(planetPosition, col.attachedRigidbody);
            }
        }

        private Collider2D[] GetCollidersInCircle(Vector2 centerPosition, float radius, string layerTag)
        {
            var hitColliders = new Collider2D[20];
            var filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask(layerTag));
            filter.useTriggers = false;
            var count = Physics2D.OverlapCircle(centerPosition, radius, filter, hitColliders);

            return count == 0 ? Array.Empty<Collider2D>() : hitColliders.Take(count).ToArray();
        }

        private void CreateOrbitMotion(Vector2 centerPos, Rigidbody2D targetRb)
        {
            targetRb.simulated = false;

            var dirToCenter = centerPos - targetRb.position;
            var targetPos = targetRb.position + dirToCenter * 2;

            // 移動モーション
            LMotion.Create(targetRb.position, targetPos, 0.6f)
                .WithEase(Ease.InSine)
                .BindToLocalPositionXY(targetRb.transform);

            // 拡大縮小モーション(手前に公転してるイメージ)
            LMotion.Create(targetRb.transform.localScale, targetRb.transform.localScale * 1.5f, 0.3f)
                .WithLoops(2, LoopType.Yoyo)
                .WithEase(Ease.Linear)
                .WithOnComplete(() =>
                {
                    targetRb.simulated = true;
                    _isOrbiting = false;
                    if (!targetRb.CompareTag("Player")) return;
                    _player.CheckBuried();
                    AddImpulse(dirToCenter.normalized * _planetParams.ReleaseForce);
                })
                .BindToLocalScale(targetRb.transform);
        }

        private void AddImpulse(Vector2 toPos)
        {
            _player.Rb.linearVelocity = Vector2.zero;
            _player.Rb.AddForce(toPos, ForceMode2D.Impulse);
        }
    }
}