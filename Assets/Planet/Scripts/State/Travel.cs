using Orbit.Core.StateMachine;
using VContainer;
using UnityEngine;

namespace Orbit.Planet.State
{
    public class Travel : IState<PlanetStateMachine>
    {
        private readonly PlanetParams _planetParams;
        private float _time;
        

        [Inject]
        public Travel(PlanetParams planetParams)
        {
            _planetParams = planetParams;
        }

        public void Enter()
        {
            _time = 0;
        }

        public void Update(PlanetStateMachine stateMachine)
        {
            _time += Time.deltaTime;
            if (_time >= _planetParams.LaunchTime) // Orbit完了後にDeployへ遷移
                stateMachine.TransitionTo(stateMachine.Deploy);
        }

        public void Exit()
        {
            
        }
    }
}