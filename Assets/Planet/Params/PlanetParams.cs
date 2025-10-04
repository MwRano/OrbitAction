using UnityEngine;

[CreateAssetMenu(fileName = "PlanetParams", menuName = "Scriptable Objects/PlanetParams")]
public class PlanetParams : ScriptableObject
{   
    [Header("惑星の追従")]
    [SerializeField] private float smoothTime;
    [SerializeField] private float maxSpeed;
    
    [Header("惑星の飛距離")] 
    [SerializeField] private float launchDistance; // 惑星の飛距離
    
    [Header("惑星の引力")]
    [SerializeField] private float attractionForce;　// 惑星の引力
    [SerializeField] private float attractionRange;　// 惑星の引力が及ぶ範囲
    
    [Header("公転")]
    [SerializeField] private float orbitalPeriod; // 公転周期
    [SerializeField] private float orbitalRange; // 公転可能な範囲
    
    public float SmoothTime => smoothTime;
    public float MaxSpeed => maxSpeed;
    public float LaunchDistance => launchDistance;
    public float AttractionForce => attractionForce;
    public float AttractionRange => attractionRange;
    public float OrbitalPeriod => orbitalPeriod;
    public float OrbitalRange => orbitalRange;
}
