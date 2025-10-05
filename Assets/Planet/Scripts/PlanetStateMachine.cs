#nullable enable
using VContainer;

public class PlanetStateMachine
{
    private IPlanetState _currentState = null!;
    
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
        _currentState = startingState;
        startingState.Enter(planet);
    }

    public void TransitionTo(IPlanetState nextState, IPlanetContext planet)
    {
        _currentState.Exit();
        _currentState = nextState;
        nextState.Enter(planet);
    }

    public void Update(IPlanetContext planet)
    {
        _currentState.Update(planet, this);
    }
}
