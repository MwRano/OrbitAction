using UnityEngine;

namespace Planet
{
    public interface IPlanetContext
    {
        GameObject OrbitAreaView { get; }
        SpriteRenderer OrbitAreaSpriteRenderer { get; }
        Transform PlanetTransform { get; }
        bool IsLaunched { get; }
    }
}