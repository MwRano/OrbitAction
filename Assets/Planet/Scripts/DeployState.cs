#nullable enable
using System;
using LitMotion;
using LitMotion.Extensions;
using Player;
using UnityEngine;
using VContainer;
using System.Linq;


namespace Planet
{
    /// <summary>
    /// planetの設置後の状態
    /// </summary>
    public class DeployState : IPlanetState
    {
        private readonly CompositeMotionHandle _handles = new();
        private readonly PlanetParams _planetParams;
        private readonly IPlayerContext _player;
        private MotionHandle _floatingMotion;
        private bool _isOrbiting;
        private GameObject _orbitAreaView = null!;

        [Inject]
        public DeployState(PlanetParams planetParams, PlayerController player)
        {
            _player = player;
            _planetParams = planetParams;
        }

        public void Enter(IPlanetContext planet)
        {
            // 浮遊モーション
            _floatingMotion = LMotion
                .Create(planet.PlanetTransform.position.y, planet.PlanetTransform.position.y - 0.2f, 1f)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToPositionY(planet.PlanetTransform)
                .AddTo(planet.PlanetTransform);
            
            // 公転範囲表示
            _orbitAreaView = planet.OrbitAreaView;

            // 拡大モーション
            float baseRadius = planet.OrbitAreaSpriteRenderer.sprite.bounds.extents.x;
            _orbitAreaView.SetActive(true);
            LMotion.Create(Vector2.zero, Vector2.one * (_planetParams.OrbitalRange / baseRadius), 0.3f)
                .WithEase(Ease.OutBack)
                .BindToLocalScaleXY(_orbitAreaView.transform)
                .AddTo(_handles);
        }

        public void Update(IPlanetContext planet, PlanetStateMachine stateMachine)
        {
            // 状態遷移の判定
            if (!planet.IsLaunched && !_player.IsGoalReached) stateMachine.TransitionTo(stateMachine.Follow, planet);
        }

        public void Exit()
        {
            _floatingMotion.Cancel();

            // 縮小モーション
            LMotion.Create((Vector2)_orbitAreaView.transform.localScale, Vector2.zero, 0.3f)
                .WithEase(Ease.OutBack)
                .WithOnComplete(() => _orbitAreaView.SetActive(false))
                .BindToLocalScaleXY(_orbitAreaView.transform)
                .AddTo(_handles);
        }
        
        public void Orbit(Vector2 planetPosition)
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
            
            return  count == 0 ? Array.Empty<Collider2D>() : hitColliders.Take(count).ToArray();
        }

        private void CreateOrbitMotion(Vector2 centerPos, Rigidbody2D targetRb)
        {
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
                    _isOrbiting = false;
                    if (!targetRb.CompareTag("Player")) return;
                    _player.SetIsSimulated(true);
                    _player.Rigidbody.linearVelocity = Vector2.zero;
                    targetRb.AddForce(dirToCenter.normalized * _planetParams.ReleaseForce, ForceMode2D.Impulse);　// 公転終了時に外周方向へ力を加える
                })
                .BindToLocalScale(targetRb.transform);
            
            if (targetRb.CompareTag("Player")) _player.SetIsSimulated(false);

        }
    }
}