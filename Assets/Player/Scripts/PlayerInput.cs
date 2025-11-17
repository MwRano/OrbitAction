using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using System;

namespace Player
{
    public class PlayerInput : IDisposable
    {
        private readonly InputSystemActions _inputSystemActions;
        private readonly ReactiveProperty<Vector2> _look = new();
        private readonly ReactiveProperty<Vector2> _aim = new();
        private readonly ReactiveProperty<bool> _jump = new();
        private readonly ReactiveProperty<bool> _orbit = new();
        
        public Vector2 Move { get; private set; }
        public ReadOnlyReactiveProperty<Vector2> Look => _look;
        public ReadOnlyReactiveProperty<Vector2> Aim=> _aim;
        public ReadOnlyReactiveProperty<bool> Jump => _jump;
        public ReadOnlyReactiveProperty<bool> Orbit => _orbit;

        [Inject]
        public PlayerInput(InputSystemActions inputSystemActions)
        {
            _inputSystemActions = inputSystemActions;

            _inputSystemActions.Player.Move.performed += OnMove;
            _inputSystemActions.Player.Move.canceled += OnMove;
            _inputSystemActions.Player.Look.performed += OnLook;
            _inputSystemActions.Player.Aim.performed += OnAim;
            _inputSystemActions.Player.Jump.performed += OnJump;
            _inputSystemActions.Player.Jump.canceled += OnJump;
            _inputSystemActions.Player.Orbit.performed += OnOrbit;
            _inputSystemActions.Player.Orbit.canceled += OnOrbit;

            _inputSystemActions.Player.Enable();
        }

        public void Dispose()
        {
            _inputSystemActions.Player.Move.performed -= OnMove;
            _inputSystemActions.Player.Move.canceled -= OnMove;
            _inputSystemActions.Player.Look.performed -= OnLook;
            _inputSystemActions.Player.Aim.performed -= OnAim;
            _inputSystemActions.Player.Jump.performed -= OnJump;
            _inputSystemActions.Player.Jump.canceled -= OnJump;
            _inputSystemActions.Player.Orbit.performed -= OnOrbit;
            _inputSystemActions.Player.Orbit.canceled -= OnOrbit;

            _inputSystemActions.Player.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            _look.Value = context.ReadValue<Vector2>();
        }

        private void OnAim(InputAction.CallbackContext context)
        {
            _aim.Value = context.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            _jump.Value = context.ReadValueAsButton();
        }

        private void OnOrbit(InputAction.CallbackContext context)
        {
            _orbit.Value = context.ReadValueAsButton();
        }
    }
}