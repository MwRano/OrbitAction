#nullable enable
using VContainer;

/// <summary>
/// planetの状態を管理するクラス
/// </summary>
public class PlanetStateMachine
{
    public IPlanetState CurrentState { get; private set; } = null!;
    public HoverState Hover { get; }
    public FollowState Follow { get; }
    public TravelState Travel { get; }
    public DeployState Deploy { get; }
    
    [Inject]
    public PlanetStateMachine(
        HoverState hoverState,
        FollowState followState,
        TravelState travelState,
        DeployState deployState)
    {
        Hover = hoverState;
        Follow = followState;
        Travel = travelState;
        Deploy = deployState;
    }

    public void Initialize(IPlanetState startingState, IPlanetContext planet)
    {
        CurrentState = startingState;
        startingState.Enter(planet);
    }

    public void TransitionTo(IPlanetState nextState, IPlanetContext planet)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter(planet);
    }

    public void Update(IPlanetContext planet)
    {
        CurrentState.Update(planet, this);
    }
}