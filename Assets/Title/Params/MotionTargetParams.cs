#nullable enable
using UnityEngine;
using System.Collections.Generic;

namespace Title
{
    /// <summary>
    /// title画面のモーション対象のオブジェクト管理
    /// </summary>
    [CreateAssetMenu(fileName = "MotionTargetParam", menuName = "Scriptable Objects/MotionTargetParam")]
    public class TitleMotionTargets : ScriptableObject
    {
        [SerializeField] private GameObject planetObject = null!;
        [SerializeField] private List<GameObject> titleObjects = null!;
        
        public GameObject PlanetObject => planetObject;
        public IEnumerable<GameObject> TitleObjects => titleObjects;
    }
}
