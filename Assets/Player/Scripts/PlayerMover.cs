using R3;
using UnityEngine;
using VContainer;

namespace Orbit.Player
{
    public class PlayerMover
    {
        private readonly PlayerCore _player;
        private readonly PlayerParam _playerParams;
    
        [Inject]
        public PlayerMover(
            PlayerParam playerParam,
            PlayerCore playerCore,
            PlayerInput playerInput)
        {
            _playerParams = playerParam;
            _player = playerCore;

            playerInput.Jump
                .Where(isJump => isJump)
                .Subscribe(_ => Jump())
                .AddTo(_player);

            Observable.EveryUpdate(UnityFrameProvider.FixedUpdate)
                .Where(_ => playerInput.Move.sqrMagnitude > 0 ||
                            _player.Rb.linearVelocity.sqrMagnitude <= 0)
                .Subscribe(_ => Move(playerInput.Move))
                .AddTo(_player);
        }

        private void Move(Vector2 moveInput)
        {
            _player.Rb.linearVelocity =
                new Vector2(moveInput.x * _playerParams.MoveSpeed, _player.Rb.linearVelocity.y);

            // 向きに応じてviewの反転
            _player.Sprite.flipX = moveInput.x < 0 || !(moveInput.x > 0) && _player.Sprite.flipX;
        }

        private void Jump()
        {
            if (!_player.IsGrounded.CurrentValue) return;
            _player.Rb.linearVelocity = new Vector2(_player.Rb.linearVelocity.x, 0);
            _player.Rb.AddForce(Vector2.up * _playerParams.JumpForce, ForceMode2D.Impulse);
        }

        public void AddImpulse(Vector2 toPos)
        {
            _player.Rb.linearVelocity = Vector2.zero;
            _player.Rb.AddForce(toPos, ForceMode2D.Impulse);
        }  
    }
}