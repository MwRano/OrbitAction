using UnityEngine;
using R3;
using VContainer;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class PlayerCore : MonoBehaviour
    {
        private PlayerParam _playerParams;

        public Rigidbody2D Rb => GetComponent<Rigidbody2D>();
        public SpriteRenderer Sprite => GetComponent<SpriteRenderer>();
        public Animator Anim => GetComponent<Animator>();
        public ReactiveProperty<bool> IsDead { get; } = new();
        public ReactiveProperty<bool> IsGoalReached { get; } = new();
        public ReactiveProperty<bool> IsGrounded { get; private set; } = new();

        private void Update()
        {
            IsGrounded.Value = CheckGrounded();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("DeadZone"))
            {
                IsDead.Value = true;
            }
            else if (other.gameObject.CompareTag("Goal"))
            {
                IsGoalReached.Value = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            IsDead.Value = false;
        }

        [Inject]
        public void Construct(PlayerParam playerParam)
        {
            _playerParams = playerParam;
        }

        public void CheckBuried()
        {
            var isBuried = Physics2D.OverlapCircle(
                transform.position,
                _playerParams.GroundCheckRadius,
                _playerParams.GroundLayer);
            if (!isBuried) return;

            IsDead.Value = true;
        }

        private bool CheckGrounded()
        {
            var groundCheckPosition = (Vector2)transform.position + _playerParams.GroundCheckOffset;
            return Physics2D.OverlapCircle(groundCheckPosition,
                _playerParams.GroundCheckRadius,
                _playerParams.GroundLayer);
        }
    }
}