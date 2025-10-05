#nullable enable
using LitMotion;
using LitMotion.Extensions;

public class DeployState : IPlanetState
{
    MotionHandle _floatingMotion;
    public void Enter(IPlanetContext planet)
    {
        _floatingMotion = LMotion.Create(planet.PlanetTransform.position.y, planet.PlanetTransform.position.y - 0.2f, 1f)
            .WithEase(Ease.InOutSine)
            .WithLoops(-1, LoopType.Yoyo)
            .BindToPositionY(planet.PlanetTransform)
            .AddTo(planet.PlanetTransform);
    }

    public void Update(IPlanetContext planet, PlanetStateMachine stateMachine)
    {
        
    }

    public void Exit()
    {
        
    }
}
