using System;
using UnityEngine;

namespace Orbit.Player
{
    [CreateAssetMenu(fileName = "PlayerEffectParams", menuName = "Scriptable Objects/PlayerEffectScriptableObject")]
    public class PlayerEffectParams : ScriptableObject
    {
        [SerializeField] private PlayerEffectData[] playerEffects;

        public PlayerEffectData[] PlayerEffects => playerEffects;
    }

    [Serializable]
    public class PlayerEffectData
    {
        public PlayerEffectType EffectType;
        public Vector2 SpawnOffset;
        public GameObject EffectPrefab;
    }

    public enum PlayerEffectType
    {
        Land,
        Slip
    }
}