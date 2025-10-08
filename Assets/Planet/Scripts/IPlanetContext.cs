using UnityEngine;

public interface IPlanetContext
{
    GameObject AttractionAreaView { get; }
    SpriteRenderer AttractionAreaSpriteRenderer { get; }
    Transform PlanetTransform { get; }
    bool IsLaunched { get; }
}