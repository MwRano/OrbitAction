namespace Core.StateMachine
{
    public interface IState<in T> where T : class
    {
        public void Enter();
        public void Update(T stateMachine);
        public void Exit();
    }
}
