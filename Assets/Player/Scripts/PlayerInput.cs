using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Player
{
    public class PlayerInput
    {
        private readonly InputSystemActions _inputSystemActions;

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

        public Vector2 Move { get; private set; } = new();
        public ReactiveProperty<Vector2> Look { get; } = new();
        public ReactiveProperty<Vector2> Aim { get; } = new();
        public ReactiveProperty<bool> Jump { get; } = new();
        public ReactiveProperty<bool> Orbit { get; } = new();

        private void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            Look.Value = context.ReadValue<Vector2>();
        }

        private void OnAim(InputAction.CallbackContext context)
        {
            Aim.Value = context.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            Jump.Value = context.ReadValueAsButton();
        }

        private void OnOrbit(InputAction.CallbackContext context)
        {
            Orbit.Value = context.ReadValueAsButton();
        }
    }
}