namespace Planet
{
    public interface IPlanetState
    {
        public void Enter(PlanetController planet);
        public void Update(PlanetController planet, PlanetStateMachine stateMachine);
        public void Exit();
    }
}