#nullable enable
using LitMotion;
using LitMotion.Extensions;

public class DeployState : IPlanetState
{
    private MotionHandle _floatingMotion;
    
    public void Enter(IPlanetContext planet)
    {
        // 浮遊モーション
        _floatingMotion = LMotion.Create(planet.PlanetTransform.position.y, planet.PlanetTransform.position.y - 0.2f, 1f)
            .WithEase(Ease.InOutSine)
            .WithLoops(-1, LoopType.Yoyo)
            .BindToPositionY(planet.PlanetTransform)
            .AddTo(planet.PlanetTransform);
    }

    public void Update(IPlanetContext planet, PlanetStateMachine stateMachine)
    {
        // 状態遷移の判定
        if(!planet.IsLaunched) stateMachine.TransitionTo(stateMachine.Follow, planet);
    }

    public void Exit()
    {
        _floatingMotion.Cancel();
    }
}
