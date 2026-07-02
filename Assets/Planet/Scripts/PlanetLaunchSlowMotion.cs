using System;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Orbit.Planet
{
    public class PlanetLaunchSlowMotion : IStartable, IDisposable
    {
        private readonly CompositeDisposable _disposable = new();
        private readonly PlanetParams _planetParams;
        private readonly float _defaultFixedDeltaTime;
        private readonly float _defaultTimeScale;

        [Inject]
        public PlanetLaunchSlowMotion(PlanetInput planetInput, PlanetParams planetParams)
        {
            _planetParams = planetParams;
            _defaultFixedDeltaTime = Time.fixedDeltaTime;
            _defaultTimeScale = Time.timeScale;

            planetInput.IsPreparingLaunch
                .Subscribe(SetSlowMotion)
                .AddTo(_disposable);
        }

        public void Start()
        {
            // Do nothing
        }

        public void Dispose()
        {
            SetSlowMotion(false);
            _disposable.Dispose();
        }

        private void SetSlowMotion(bool isSlow)
        {
            if (isSlow)
            {
                var timeScale = Mathf.Clamp(_planetParams.LaunchAimTimeScale, 0.05f, 1f);
                Time.timeScale = timeScale;
                Time.fixedDeltaTime = _defaultFixedDeltaTime * timeScale;
                return;
            }

            Time.timeScale = _defaultTimeScale;
            Time.fixedDeltaTime = _defaultFixedDeltaTime;
        }
    }
}
