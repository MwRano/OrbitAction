using UnityEngine;

namespace Orbit.Game
{
    [CreateAssetMenu(fileName = "FadeParms", menuName = "Scriptable Objects/FadeParms")]
    public class FadeParams : ScriptableObject
    {
        [SerializeField] private Material fadeMat;
        public Material FadeMat => fadeMat;
    }
}