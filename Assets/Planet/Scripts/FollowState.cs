#nullable enable
using LitMotion;
using LitMotion.Extensions;
using Player;
using UnityEngine;
using VContainer;

namespace Planet
{
    public class FollowState : IPlanetState
    {
        private readonly float _maxSpeed;
        private readonly PlayerController _player;
        private readonly float _smoothTime;
        private Vector2 _currentVelocity;
        private MotionHandle _rotationMotion;

        [Inject]
        public FollowState(PlayerController player, PlanetParams planetParams)
        {
            _player = player;
            _smoothTime = planetParams.SmoothTime;
            _maxSpeed = planetParams.MaxSpeed;
        }

        public void Enter(PlanetController planet)
        {
            if (_rotationMotion.IsActive()) return;

            _rotationMotion = LMotion.Create(0f, 360f, 30f)
                .WithEase(Ease.Linear)
                .WithLoops(-1)
                .BindToLocalEulerAnglesZ(planet.PlanetTransform)
                .AddTo(planet.PlanetTransform);
        }

        public void Update(PlanetController planet, PlanetStateMachine stateMachine)
        {
            Vector2 playerPos = _player.PlayerTransform.position;
            Vector2 targetPosition = _player.IsFacingRight　// playerの向きに応じて追従位置を変更
                ? new Vector2(playerPos.x - 1, playerPos.y + 1)
                : new Vector2(playerPos.x + 1, playerPos.y + 1);

            planet.PlanetTransform.position = Vector2.SmoothDamp(
                planet.PlanetTransform.position,
                targetPosition,
                ref _currentVelocity,
                _smoothTime,
                _maxSpeed
            );

            // 状態遷移の判定
            if (_player.Rigidbody.linearVelocity.sqrMagnitude < 0.01f &&
                _currentVelocity.sqrMagnitude < 0.01f) // playerが停止したらIdleへ遷移
            {
                stateMachine.TransitionTo(stateMachine.Hover, planet);
            }
            else if (planet.IsLaunched || _player.IsGoalReached) // 発射されたらTravelへ遷移
            {
                stateMachine.TransitionTo(stateMachine.Travel, planet);
            }
        }

        public void Exit()
        {
        }
    }
}