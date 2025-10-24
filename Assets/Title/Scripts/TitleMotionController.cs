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
        
        void Awake()
        {
            // 自転モーション
            LMotion.Create(0f, 360f, 120f)
                .WithEase(Ease.Linear)
                .WithLoops(-1)
                .BindToLocalEulerAnglesZ(planetObject.transform)
                .AddTo(planetObject.transform);
            
            titleObjects.Add(planetObject);
            foreach (var obj in titleObjects)
            {
                // 浮遊モーション
                float randomDuration = Random.Range(1f, 2f);
                LMotion.Create(obj.transform.position.y, obj.transform.position.y - 0.2f, randomDuration)
                    .WithEase(Ease.InOutSine)
                    .WithLoops(-1, LoopType.Yoyo)
                    .BindToPositionY(obj.transform)
                    .AddTo(obj.transform);
            }
        }
    }
}

