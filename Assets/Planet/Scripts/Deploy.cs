#nullable enable
using LitMotion;
using LitMotion.Extensions;
using Orbit.Core.StateMachine;
using Orbit.Player;
using UnityEngine;
using VContainer;

namespace Orbit.Planet
{
    /// <summary>
    /// planetの設置後の状態
    /// </summary>
    public class Deploy : IState<PlanetStateMachine>
    {
        private readonly CompositeMotionHandle _handles = new();
        private readonly PlanetParams _planetParams;
        private readonly PlayerCore _player;
        private MotionHandle _floatingMotion;
        private bool _isOrbiting;
        private GameObject _orbitAreaView = null!;
        private readonly PlanetController _planet;

        [Inject]
        public Deploy(
            PlanetParams planetParams, 
            PlayerCore player,
            PlanetController planet)
        {
            _player = player;
            _planetParams = planetParams;
            _planet = planet;
        }

        public void Enter()
        {
            // 浮遊モーション
            _floatingMotion = LMotion
                .Create(_planet.PlanetTransform.position.y, _planet.PlanetTransform.position.y - 0.2f, 1f)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToPositionY(_planet.PlanetTransform)
                .AddTo(_planet.PlanetTransform);

            // 公転範囲表示
            _orbitAreaView = _planet.OrbitAreaView;

            // 拡大モーション
            float baseRadius = _planet.OrbitAreaSpriteRenderer.sprite.bounds.extents.x;
            _orbitAreaView.SetActive(true);
            LMotion.Create(Vector2.zero, Vector2.one * (_planetParams.OrbitalRange / baseRadius), 0.3f)
                .WithEase(Ease.OutBack)
                .BindToLocalScaleXY(_orbitAreaView.transform)
                .AddTo(_handles);
        }

        public void Update(PlanetStateMachine stateMachine)
        {
            // 状態遷移の判定
            if (!_planet.IsLaunched && !_player.IsGoalReached.CurrentValue)
                stateMachine.TransitionTo(stateMachine.Follow);
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