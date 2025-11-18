using System;
using R3;
using UnityEngine.InputSystem;
using VContainer;

namespace Orbit.Planet
{
    public class PlanetInput :IDisposable
    {
        private readonly InputSystemActions _inputSystemActions;
        private readonly ReactiveProperty<bool> _orbit = new();
        private readonly ReactiveProperty<bool> _launch = new();
        
        public ReadOnlyReactiveProperty<bool> Orbit => _orbit;
        public ReadOnlyReactiveProperty<bool> Launch => _launch;

        [Inject]
        public PlanetInput(InputSystemActions inputSystemActions)
        {
            _inputSystemActions = inputSystemActions;
            
            _inputSystemActions.Planet.Orbit.performed += OnOrbit;
            _inputSystemActions.Planet.Orbit.canceled += OnOrbit;
            _inputSystemActions.Planet.Launch.performed += OnLaunch;

            _inputSystemActions.Planet.Enable();
        }

        public void Dispose()
        {
            _inputSystemActions.Planet.Orbit.performed -= OnOrbit;
            _inputSystemActions.Planet.Orbit.canceled -= OnOrbit;
            _inputSystemActions.Planet.Launch.performed -= OnLaunch;

            _inputSystemActions.Planet.Disable();
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