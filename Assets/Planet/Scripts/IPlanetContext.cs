using UnityEngine;

namespace Planet
{
    public interface IPlanetContext
    {
        SpriteRenderer PlanetSpriteRenderer { get; }
        GameObject OrbitAreaView { get; }
        SpriteRenderer OrbitAreaSpriteRenderer { get; }
        Transform PlanetTransform { get; }
        bool IsLaunched { get; }
    }
}