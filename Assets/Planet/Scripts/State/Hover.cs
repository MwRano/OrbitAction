using LitMotion;
using LitMotion.Extensions;
using Orbit.Core.StateMachine;
using Orbit.Player;
using VContainer;

namespace Orbit.Planet.State
{
    public class Hover : IState<PlanetStateMachine>
    {
        private const float VelocityThreshold = 0.01f*0.01f;
        private readonly PlayerCore _player;
        private readonly PlanetCore _planet;
        private readonly PlanetInput _planetInput;
        private MotionHandle _floatingMotion;

        [Inject]
        public Hover(PlayerCore player, PlanetCore planet, PlanetInput planetInput)
        {
            _player = player;
            _planet = planet;
            _planetInput = planetInput;
        }

        public void Enter()
        {
            _floatingMotion = LMotion
                .Create(_planet.transform.position.y, _planet.transform.position.y - 0.2f, 1f)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToPositionY(_planet.transform)
                .AddTo(_planet);
        }

        public void Update(PlanetStateMachine stateMachine)
        {
            // 状態遷移の判定
            if (_player.Rb.linearVelocity.sqrMagnitude > VelocityThreshold) // playerが動き出したらFollowへ遷移
            {
                stateMachine.TransitionTo(stateMachine.Follow);
            }
            else if (_planetInput.Launch.CurrentValue)
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