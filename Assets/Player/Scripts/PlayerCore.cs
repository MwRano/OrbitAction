using R3;
using UnityEngine;
using VContainer;

namespace Orbit.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class PlayerCore : MonoBehaviour
    {
        private readonly ReactiveProperty<bool> _isGoalReached = new();
        private readonly ReactiveProperty<bool> _isGrounded = new();
        private PlayerParam _playerParams;

        public Rigidbody2D Rb => GetComponent<Rigidbody2D>();
        public SpriteRenderer Sprite => GetComponent<SpriteRenderer>();
        public Animator Anim => GetComponent<Animator>();
        public Collider2D Collider => GetComponent<CapsuleCollider2D>();
        public ReactiveProperty<bool> IsDead { get; } = new();
        public ReadOnlyReactiveProperty<bool> IsGoalReached => _isGoalReached;
        public ReadOnlyReactiveProperty<bool> IsGrounded => _isGrounded;

        private void Update()
        {
            _isGrounded.Value = CheckGrounded();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("DeadZone"))
            {
                IsDead.Value = true;
            }
            else if (other.gameObject.CompareTag("Goal"))
            {
                _isGoalReached.Value = true;
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