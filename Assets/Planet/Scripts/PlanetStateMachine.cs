#nullable enable
using Orbit.Core.StateMachine;
using VContainer;

namespace Orbit.Planet
{
    /// <summary>
    /// planetの状態を管理するクラス
    /// </summary>
    public class PlanetStateMachine : StateMachine<IState<PlanetStateMachine>, PlanetStateMachine>
    {
        [Inject]
        public PlanetStateMachine(
            Hover hover,
            Follow follow,
            Travel travel,
            Deploy deploy)
        {
            Hover = hover;
            Follow = follow;
            Travel = travel;
            Deploy = deploy;
        }
        
        public Hover Hover { get; }
        public Follow Follow { get; }
        public Travel Travel { get; }
        public Deploy Deploy { get; }

        public override void Initialize()
        {
            SetInitialState(Follow);
        }
    }
}