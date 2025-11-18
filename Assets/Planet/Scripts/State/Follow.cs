using LitMotion;
using LitMotion.Extensions;
using Orbit.Core.StateMachine;
using Orbit.Player;
using UnityEngine;
using VContainer;

namespace Orbit.Planet.State
{
    public class Follow : IState<PlanetStateMachine>
    {
        private const float VelocityThreshold = 0.01f*0.01f;
        
        private readonly PlayerCore _player;
        private readonly PlanetCore _planet;
        private readonly PlanetInput _planetInput;
        private readonly PlanetParams _planetParams;
        private Vector2 _currentVelocity;
        private MotionHandle _rotationMotion;

        [Inject]
        public Follow(
            PlayerCore player, 
            PlanetParams planetParams,
            PlanetCore planet,
            PlanetInput planetInput)
        {
            _player = player;
            _planetParams = planetParams;
            _planet = planet;
            _planetInput = planetInput;
        }

        public void Enter()
        {
            if (_rotationMotion.IsActive()) return;

            _rotationMotion = LMotion.Create(0f, 360f, 30f)
                .WithEase(Ease.Linear)
                .WithLoops(-1)
                .BindToLocalEulerAnglesZ(_planet.transform)
                .AddTo(_planet);
        }

        public void Update(PlanetStateMachine stateMachine)
        {
            _planet.transform.position = CalculateFollowPosition();

            // 状態遷移の判定
            if (_player.Rb.linearVelocity.sqrMagnitude < VelocityThreshold &&
                _currentVelocity.sqrMagnitude < VelocityThreshold) // playerが停止したらIdleへ遷移
            {
                stateMachine.TransitionTo(stateMachine.Hover);
            }
            else if (_planetInput.Launch.CurrentValue) // 発射されたらTravelへ遷移
            {
                stateMachine.TransitionTo(stateMachine.Travel);
            }
        }

        public void Exit()
        {
        }
        
        private Vector2 CalculateFollowPosition() 
        {
            Vector2 playerPos = _player.transform.position;
            var targetPos = _player.Sprite.flipX　// playerの向きに応じて追従位置を変更
                ? new Vector2(playerPos.x + 1, playerPos.y + 1)
                : new Vector2(playerPos.x - 1, playerPos.y + 1);

            return Vector2.SmoothDamp(
                _planet.transform.position,
                targetPos,
                ref _currentVelocity,
                _planetParams.SmoothTime,
                _planetParams.MaxSpeed
            );
        }
    }
}