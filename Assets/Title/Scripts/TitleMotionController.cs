#nullable enable
using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

namespace Title
{
    /// <summary>
    /// title画面のモーション制御
    /// </summary>
    public class TitleMotionController : MonoBehaviour
    {
        [SerializeField] private GameObject planetObject = null!;
        [SerializeField] private List<GameObject> titleObjects = null!;
        
        public void Awake()
        {
            // 自転モーション
            AddRotateMotion(planetObject.transform);
            
            foreach (var obj in titleObjects)
            {
                AddFloatMotion(obj.transform);
            }
            
            AddFloatMotion(planetObject.transform);
        }
        
        // 自転モーションを付与する
        private void AddRotateMotion(Transform target, float startAngle = 0f, float endAngle = 360f, float maxDuration = 120f)
        {
            LMotion.Create(startAngle, endAngle, maxDuration)
                .WithEase(Ease.Linear)
                .WithLoops(-1)
                .BindToLocalEulerAnglesZ(target)
                .AddTo(target);
        }

        // 浮遊モーションを付与する
        private void AddFloatMotion(Transform target, float offsetY = -0.2f, float minDuration = 1f, float maxDuration = 2f)
        {
            var randomDuration = Random.Range(minDuration, maxDuration);
            LMotion.Create(target.position.y, target.position.y + offsetY, randomDuration)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToPositionY(target)
                .AddTo(target);
        }
    }
}
