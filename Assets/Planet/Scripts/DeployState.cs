#nullable enable
using System;
using LitMotion;
using LitMotion.Extensions;
using Player;
using R3;
using UnityEngine;
using VContainer;

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
        private bool _isOrbiting = false;
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

        /// <summary>
        /// 公転
        /// </summary>
        public void Orbit(Vector2 planetPosition)
        {
            // 公転範囲内にPlayerがいるか判定
            if (!TryGetSinglePlayerInOrbitArea(planetPosition, out var playerCollider)
                || _isOrbiting) return;

            _isOrbiting = true;
            var playerRigidbody = playerCollider.gameObject.GetComponent<Rigidbody2D>();
            var directionToPlanet = planetPosition - playerRigidbody.position;
            var targetPos = _player.Rigidbody.position + directionToPlanet * 2; // planetと対照位置に移動するように

            // 公転モーション
            LMotion.Create(_player.Rigidbody.position, targetPos, 0.6f)
                .WithEase(Ease.InSine)
                .BindToLocalPositionXY(_player.PlayerTransform)
                .AddTo(_handles);

            // 拡大縮小モーション(手前に公転してるイメージ)
            LMotion.Create(_player.PlayerTransform.localScale, _player.PlayerTransform.localScale * 1.5f, 0.3f)
                .WithLoops(2, LoopType.Yoyo)
                .WithEase(Ease.Linear)
                .WithOnComplete(() =>
                {
                    _isOrbiting = false;
                    _player.SetCanControl(false); // 一時的に操作不可にする
                    _player.Rigidbody.linearVelocity = Vector2.zero;
                    playerRigidbody.AddForce(directionToPlanet.normalized * _planetParams.ReleaseForce,
                        ForceMode2D.Impulse);　// 公転終了時に外周方向へ力を加える
                    Observable.Timer(TimeSpan.FromSeconds(0.1f))
                        .Subscribe(_ => _player.SetCanControl(true));　// delayしてから操作可能にする
                })
                .BindToLocalScale(_player.PlayerTransform)
                .AddTo(_handles);
        }

        // 公転エリア内のPlayerを取得するメソッド
        private bool TryGetSinglePlayerInOrbitArea(Vector2 planetPosition, out Collider2D playerCollider)
        {
            Collider2D[] hitCollidersCache = new Collider2D[20];
            var filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Player"));
            filter.useTriggers = false;
            int size = Physics2D.OverlapCircle(planetPosition, _planetParams.OrbitalRange, filter, hitCollidersCache);
            if (size != 1)
            {
                if (size >= 2) Debug.LogError("Orbit範囲内にPlayerが複数存在します。");
                playerCollider = null!;
                return false;
            }

            playerCollider = hitCollidersCache[0];
            return true;
        }
    }
}