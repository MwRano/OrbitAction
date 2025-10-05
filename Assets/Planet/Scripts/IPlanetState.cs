public interface IPlanetState
{
    public void Enter(IPlanetContext planet);
    public void Update(IPlanetContext planet, PlanetStateMachine stateMachine);
    public void Exit();
}
