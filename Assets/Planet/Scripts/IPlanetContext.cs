using UnityEngine;

public interface IPlanetContext
{
    GameObject AttractionAreaView { get; }
    SpriteRenderer AttractionAreaSpriteRenderer { get; }
    GameObject OrbitAreaView { get; }
    SpriteRenderer OrbitAreaSpriteRenderer { get; }
    Transform PlanetTransform { get; }
    bool IsLaunched { get; }
}