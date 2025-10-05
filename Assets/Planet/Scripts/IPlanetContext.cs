using UnityEngine;
public interface IPlanetContext
{
    Transform PlanetTransform { get; }
    bool IsLaunched { get; }
}
