using UnityEngine;

namespace Orbit.Planet
{
    public class PlanetCore : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer orbitableAreaView = null!;
        
        public SpriteRenderer PlanetView => GetComponent<SpriteRenderer>();
        public SpriteRenderer OrbitableAreaView => orbitableAreaView;
    }
}