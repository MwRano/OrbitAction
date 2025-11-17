#nullable enable
using LitMotion;
using LitMotion.Extensions;
using Player;
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
        private readonly PlayerCore _player;
        private MotionHandle _floatingMotion;
        private bool _isOrbiting;
        private GameObject _orbitAreaView = null!;

        [Inject]
        public DeployState(PlanetParams planetParams, PlayerCore player)
        {
            _player = player;
            _planetParams = planetParams;
        }

        public void Enter(PlanetController planet)
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

        public void Update(PlanetController planet, PlanetStateMachine stateMachine)
        {
            // 状態遷移の判定
            if (!planet.IsLaunched && !_player.IsGoalReached.Value)
                stateMachine.TransitionTo(stateMachine.Follow, planet);
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
    }
}