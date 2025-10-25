#nullable enable
using LitMotion;
using LitMotion.Extensions;
using Player;
using VContainer;

namespace Planet
{
    public class HoverState : IPlanetState
    {
        private readonly IPlayerContext _player;
        private MotionHandle _floatingMotion;

        [Inject]
        public HoverState(IPlayerContext player)
        {
            _player = player;
        }

        public void Enter(IPlanetContext planet)
        {
            _floatingMotion = LMotion
                .Create(planet.PlanetTransform.position.y, planet.PlanetTransform.position.y - 0.2f, 1f)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToPositionY(planet.PlanetTransform)
                .AddTo(planet.PlanetTransform);
        }

        public void Update(IPlanetContext planet, PlanetStateMachine stateMachine)
        {
            // 状態遷移の判定
            if (_player.Rigidbody.linearVelocity.sqrMagnitude > 0.01f) // playerが動き出したらFollowへ遷移
            {
                stateMachine.TransitionTo(stateMachine.Follow, planet);
            }
            else if (planet.IsLaunched)
            {
                stateMachine.TransitionTo(stateMachine.Travel, planet);
            }
        }

        public void Exit()
        {
            _floatingMotion.Cancel();
        }
    }
}