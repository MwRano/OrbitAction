using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

namespace Title
{
    public class TitlePlanetController : MonoBehaviour
    {
        void Start()
        {
            // 自転モーション
            LMotion.Create(0f, 360f, 120f)
                .WithEase(Ease.Linear)
                .WithLoops(-1)
                .BindToLocalEulerAnglesZ(transform)
                .AddTo(transform);
        }
        
    }
}

