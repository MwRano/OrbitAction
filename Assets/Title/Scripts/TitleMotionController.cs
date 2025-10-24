#nullable enable
using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using VContainer;
using VContainer.Unity;

namespace Title
{
    /// <summary>
    /// title画面のモーション制御
    /// </summary>
    public class TitleMotionController : IStartable
    {
        private readonly GameObject _planetObject;
        private readonly IEnumerable<GameObject> _titleObjects;
        
        [Inject]
        public TitleMotionController(TitleMotionTargets targets)
        {
            _planetObject = targets.PlanetObject;
            _titleObjects = targets.TitleObjects;
        }
        
        public void Start()
        {
            // 自転モーション
            AddRotateMotion(_planetObject.transform);
            
            foreach (var obj in _titleObjects)
            {
                AddFloatMotion(obj.transform);
            }
            
            AddFloatMotion(_planetObject.transform);
        }
        
        // 自転モーションを付与する
        private void AddRotateMotion(Transform target, float startAngle = 0f, float endAngle = 360f, float maxDuration = 120f)
        {
            LMotion.Create(startAngle, endAngle, maxDuration)
                .WithEase(Ease.Linear)
                .WithLoops(-1)
                .BindToLocalEulerAnglesZ(target.transform)
                .AddTo(target.transform);
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
