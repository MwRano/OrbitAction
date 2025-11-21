using LitMotion;
using UnityEngine;
using VContainer;

namespace Orbit.Game
{
    public class ScreenFader
    {
        private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");
        private static readonly int FlipX = Shader.PropertyToID("_FlipX");
        private readonly FadeParams _fadeParams;

        [Inject]
        public ScreenFader(FadeParams fadeParams)
        {
            _fadeParams = fadeParams;
        }

        public void FadeIn()
        {
            LMotion.Create(1.0f, 0f, 0.5f)
                .WithEase(Ease.Linear)
                .Bind(value => _fadeParams.FadeMat.SetFloat(FadeAmount, value));
        }

        public void FadeOut()
        {
            _fadeParams.FadeMat.SetInt(FlipX, 1);
            LMotion.Create(0, 1.0f, 0.5f)
                .WithEase(Ease.Linear)
                .WithOnComplete(() => _fadeParams.FadeMat.SetInt(FlipX, 0))
                .Bind(value => _fadeParams.FadeMat.SetFloat(FadeAmount, value));
        }
    }
}