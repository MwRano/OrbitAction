using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace Utility
{
    public class MotionCreator
    {
        public static MotionHandle CreateFloatingMotion(Transform targetTransform, float amplitude, float duration)
        {
            return LMotion
                .Create(targetTransform.position.y -amplitude, targetTransform.position.y + amplitude, duration)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToPositionY(targetTransform)
                .AddTo(targetTransform);
        }
    }
}
