using UnityEngine;

namespace Orbit.Planet
{
    [CreateAssetMenu(fileName = "PlanetParams", menuName = "Scriptable Objects/PlanetParams")]
    public class PlanetParams : ScriptableObject
    {
        [Header("追従")] 
        [SerializeField] private float smoothTime;
        [SerializeField] private float maxSpeed;

        [Header("発射")] 
        [SerializeField] private float launchDistance; // 惑星の飛距離
        [SerializeField] private float launchTime; // 発射時間

        [Header("公転")] 
        [SerializeField] private float orbitalRange; // 公転可能な範囲
        [SerializeField] private float orbitalTime;
        [SerializeField] private float releaseForce; // 公転から離脱する際の力

        public float SmoothTime => smoothTime;
        public float MaxSpeed => maxSpeed;
        public float LaunchDistance => launchDistance;
        public float LaunchDistanceSqr => launchDistance * launchDistance;
        public float LaunchTime => launchTime;
        public float OrbitalRange => orbitalRange;
        public float OrbitalTime => orbitalTime;
        public float ReleaseForce => releaseForce;
    }
}