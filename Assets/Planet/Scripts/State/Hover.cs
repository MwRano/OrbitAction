#nullable enable
using LitMotion;
using LitMotion.Extensions;
using Orbit.Core.StateMachine;
using Orbit.Player;
using VContainer;

namespace Orbit.Planet.State
{
    public class Hover : IState<PlanetStateMachine>
    {
        private readonly PlayerCore _player;
        private MotionHandle _floatingMotion;
        private readonly PlanetController _planet;

        [Inject]
        public Hover(PlayerCore player, PlanetController planet)
        {
            _player = player;
            _planet = planet;
        }

        public void Enter()
        {
            _floatingMotion = LMotion
                .Create(_planet.PlanetTransform.position.y, _planet.PlanetTransform.position.y - 0.2f, 1f)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToPositionY(_planet.PlanetTransform)
                .AddTo(_planet.PlanetTransform);
        }

        public void Update(PlanetStateMachine stateMachine)
        {
            // 状態遷移の判定
            if (_player.Rb.linearVelocity.sqrMagnitude > 0.01f) // playerが動き出したらFollowへ遷移
            {
                stateMachine.TransitionTo(stateMachine.Follow);
            }
            else if (_planet.IsLaunched)
            {
                stateMachine.TransitionTo(stateMachine.Travel);
            }
        }

        public void Exit()
        {
            _floatingMotion.Cancel();
        }
    }
}