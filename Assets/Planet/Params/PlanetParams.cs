using UnityEngine;

namespace Planet
{
    [CreateAssetMenu(fileName = "PlanetParams", menuName = "Scriptable Objects/PlanetParams")]
    public class PlanetParams : ScriptableObject
    {
        [Header("惑星の追従")] [SerializeField] private float smoothTime;

        [SerializeField] private float maxSpeed;

        [Header("惑星の飛距離")] [SerializeField] private float launchDistance; // 惑星の飛距離

        [Header("公転")] [SerializeField] private float orbitalRange; // 公転可能な範囲

        [SerializeField] private float releaseForce; // 公転から離脱する際の力

        public float SmoothTime => smoothTime;
        public float MaxSpeed => maxSpeed;
        public float LaunchDistance => launchDistance;
        public float ReleaseForce => releaseForce;
        public float OrbitalRange => orbitalRange;
    }
}