#nullable enable
using LitMotion;
using LitMotion.Extensions;
using Orbit.Core.StateMachine;
using Orbit.Player;
using UnityEngine;
using VContainer;

namespace Orbit.Planet
{
    public class Follow : IState<PlanetStateMachine>
    {
        private readonly float _maxSpeed;
        private readonly PlayerCore _player;
        private readonly PlayerMover _playerMover;
        private readonly float _smoothTime;
        private Vector2 _currentVelocity;
        private MotionHandle _rotationMotion;
        private readonly PlanetController _planet;

        [Inject]
        public Follow(
            PlayerCore player, 
            PlanetParams planetParams,
            PlanetController planet)
        {
            _player = player;
            _smoothTime = planetParams.SmoothTime;
            _maxSpeed = planetParams.MaxSpeed;
            _planet = planet;
        }

        public void Enter()
        {
            if (_rotationMotion.IsActive()) return;

            _rotationMotion = LMotion.Create(0f, 360f, 30f)
                .WithEase(Ease.Linear)
                .WithLoops(-1)
                .BindToLocalEulerAnglesZ(_planet.PlanetTransform)
                .AddTo(_planet.PlanetTransform);
        }

        public void Update(PlanetStateMachine stateMachine)
        {
            Vector2 playerPos = _player.Rb.transform.position;
            Vector2 targetPosition = _player.Sprite.flipX　// playerの向きに応じて追従位置を変更
                ? new Vector2(playerPos.x + 1, playerPos.y + 1)
                : new Vector2(playerPos.x - 1, playerPos.y + 1);

            _planet.PlanetTransform.position = Vector2.SmoothDamp(
                _planet.PlanetTransform.position,
                targetPosition,
                ref _currentVelocity,
                _smoothTime,
                _maxSpeed
            );

            // 状態遷移の判定
            if (_player.Rb.linearVelocity.sqrMagnitude < 0.01f &&
                _currentVelocity.sqrMagnitude < 0.01f) // playerが停止したらIdleへ遷移
            {
                stateMachine.TransitionTo(stateMachine.Hover);
            }
            else if (_planet.IsLaunched || _player.IsGoalReached.CurrentValue) // 発射されたらTravelへ遷移
            {
                stateMachine.TransitionTo(stateMachine.Travel);
            }
        }

        public void Exit()
        {
        }
    }
}