#nullable enable

using Orbit.Core.StateMachine;
using VContainer;

namespace Orbit.Player.State
{
    /// <summary>
    /// Playerの状態を管理するステートマシン
    /// </summary>
    public class PlayerStateMachine : StateMachine<IState<PlayerStateMachine>, PlayerStateMachine>
    {
        [Inject]
        public PlayerStateMachine(
            Idle idle,
            Walk walk,
            Jump jump,
            Fall fall,
            Death death)
        {
            Idle = idle;
            Walk = walk;
            Jump = jump;
            Fall = fall;
            Death = death;
        }

        public Idle Idle { get; }
        public Walk Walk { get; }
        public Jump Jump { get; }
        public Fall Fall { get; }
        public Death Death { get; }

        public override void Initialize()
        {
            SetInitialState(Idle);
        }
    }
}