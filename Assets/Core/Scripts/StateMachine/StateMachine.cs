#nullable enable
using VContainer.Unity;

namespace Orbit.Core.StateMachine
{
    public abstract class StateMachine<TState, TStateMachine> : IInitializable, ITickable
        where TState : class, IState<TStateMachine>
        where TStateMachine : class
    {
        public TState CurrentState { get; private set; } = null!;

        public abstract void Initialize();

        public void Tick()
        {
            CurrentState.Update(this as TStateMachine ?? throw new System.InvalidCastException());
        }

        public void TransitionTo(TState nextState)
        {
            CurrentState?.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
        }

        protected void SetInitialState(TState initialState)
        {
            CurrentState = initialState;
            CurrentState.Enter();
        }
    }
}