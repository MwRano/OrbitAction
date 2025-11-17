namespace Planet
{
    public interface IPlanetState
    {
        public void Enter();
        public void Update(PlanetStateMachine stateMachine);
        public void Exit();
    }
}