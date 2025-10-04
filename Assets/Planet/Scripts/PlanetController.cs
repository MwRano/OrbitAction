#nullable enable
using UnityEngine;
using VContainer;


public class PlanetController : MonoBehaviour
{
    private PlayerController _player = null!;
    private float _smoothTime;
    private float _maxSpeed;

    [Inject]
    public void Construct(PlayerController playerController, PlanetParams planetParams)
    {
        _player = playerController;
        _smoothTime = planetParams.SmoothTime;
        _maxSpeed = planetParams.MaxSpeed;
    }
}
