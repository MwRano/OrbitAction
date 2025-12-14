using System;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using Orbit.Player;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Orbit.Planet
{
    public class PlanetSkill : ITickable
    {
        private readonly DeployPositionCalculator _deployPositionCalculator;
        private readonly PlanetCore _planet;
        private readonly PlanetParams _planetParams;
        private readonly PlayerCore _player;
        private readonly PlayerAimer _playerAimer;
        private readonly PlayerMover _playerMover;
        private bool _isOrbiting;

        [Inject]
        public PlanetSkill(
            PlanetParams planetParams,
            PlanetCore planet,
            PlayerCore player,
            PlayerMover playerMover,
            PlanetInput planetInput,
            DeployPositionCalculator deployPositionCalculator,
            PlayerAimer playerAimer,
            PlanetStateMachine planetStateMachine)
        {
            _planetParams = planetParams;
            _planet = planet;
            _player = player;
            _playerMover = playerMover;
            _deployPositionCalculator = deployPositionCalculator;
            _playerAimer = playerAimer;

            planetInput.Orbit
                .Where(isOrbit => isOrbit &&
                                  planetStateMachine.CurrentState == planetStateMachine.Deploy)
                .Subscribe(_ => Orbit())
                .AddTo(planet);

            planetInput.Launch
                .Where(isLaunch => isLaunch &&
                                   planetStateMachine.CurrentState == planetStateMachine.Hover　||
                                   planetStateMachine.CurrentState == planetStateMachine.Follow)
                .Subscribe(_ => Launch())
                .AddTo(planet);
        }

        public void Tick()
        {
        }

        private void Launch()
        {
            // 滞空時、スキルの使いやすさのために位置固定
            if (!_player.IsGrounded.CurrentValue)
            {
                _player.Rb.constraints = RigidbodyConstraints2D.FreezePosition;
            }

            // 移動モーション 
            var planetRadius = _planet.PlanetView.bounds.extents.x;
            var destPos = _deployPositionCalculator.Calculate(
                _player.transform.position,
                _playerAimer.AimDirection,
                _planet.transform.position,
                planetRadius
            );

            // deply地点にlaunch
            LMotion.Create((Vector2)_planet.transform.position, destPos, _planetParams.LaunchTime)
                .WithEase(Ease.OutCubic)
                .WithOnComplete(() =>
                {
                    Observable.Timer(TimeSpan.FromSeconds(0.5f))
                        .Subscribe(_ =>
                        {
                            _player.Rb.constraints = RigidbodyConstraints2D.None
                                                     | RigidbodyConstraints2D.FreezeRotation;
                            _player.Rb.WakeUp();
                        })
                        .AddTo(_player);
                })
                .BindToPositionXY(_planet.transform)
                .AddTo(_planet);
        }

        private void Orbit()
        {
            var colliders = GetCollidersInCircle(
                _planet.transform.position,
                _planetParams.OrbitalRange,
                "Orbitable");
            if (colliders.Length == 0 || _isOrbiting) return;

            _isOrbiting = true;
            foreach (var col in colliders)
            {
                CreateOrbitMotion(_planet.transform.position, col.attachedRigidbody);
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
            LMotion.Create(targetRb.position, targetPos, _planetParams.OrbitalTime)
                .WithEase(Ease.InSine)
                .BindToLocalPositionXY(targetRb.transform);

            // 拡大縮小モーション(手前に公転してるイメージ)
            LMotion.Create(targetRb.transform.localScale, targetRb.transform.localScale * 1.5f,
                    _planetParams.OrbitalTime / 2)
                .WithLoops(2, LoopType.Yoyo)
                .WithEase(Ease.Linear)
                .WithOnComplete(() =>
                {
                    targetRb.simulated = true;
                    _isOrbiting = false;
                    if (!targetRb.CompareTag("Player")) return;
                    _player.CheckBuried();
                    _playerMover.AddImpulse(dirToCenter.normalized * _planetParams.ReleaseForce);
                })
                .BindToLocalScale(targetRb.transform);
        }
    }
}