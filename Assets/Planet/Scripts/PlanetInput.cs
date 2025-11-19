using System;
using R3;
using UnityEngine.InputSystem;
using VContainer;

namespace Orbit.Planet
{
    public class PlanetInput : IDisposable
    {
        private readonly InputSystemActions _inputSystemActions;
        private readonly ReactiveProperty<bool> _launch = new();
        private readonly ReactiveProperty<bool> _orbit = new();

        [Inject]
        public PlanetInput(InputSystemActions inputSystemActions)
        {
            _inputSystemActions = inputSystemActions;

            _inputSystemActions.Planet.Orbit.performed += OnOrbit;
            _inputSystemActions.Planet.Orbit.canceled += OnOrbit;
            _inputSystemActions.Planet.Launch.performed += OnLaunch;

            _inputSystemActions.Planet.Enable();
        }

        public ReadOnlyReactiveProperty<bool> Orbit => _orbit;
        public ReadOnlyReactiveProperty<bool> Launch => _launch;

        public void Dispose()
        {
            _inputSystemActions.Planet.Orbit.performed -= OnOrbit;
            _inputSystemActions.Planet.Orbit.canceled -= OnOrbit;
            _inputSystemActions.Planet.Launch.performed -= OnLaunch;

            _inputSystemActions.Planet.Disable();
        }

        public void ResetLaunch()
        {
            _launch.Value = false;
        }

        private void OnOrbit(InputAction.CallbackContext context)
        {
            _orbit.Value = context.ReadValueAsButton();
        }

        private void OnLaunch(InputAction.CallbackContext context)
        {
            _launch.Value ^= context.ReadValueAsButton();
        }
    }
}