using UnityEngine;

namespace Orbit.Game
{
    [CreateAssetMenu(fileName = "FadeParms", menuName = "Scriptable Objects/FadeParms")]
    public class FadeParams : ScriptableObject
    {
        [SerializeField] private Material fadeMat;
        [SerializeField] private float fadeTime;
        
        public Material FadeMat => fadeMat;
        public float FadeTime => fadeTime;
    }
}