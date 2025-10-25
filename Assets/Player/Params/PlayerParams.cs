using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerParam", menuName = "Scriptable Objects/PlayerParam")]
    public class PlayerParam : ScriptableObject
    {
        [Header("移動")] [SerializeField] private float moveSpeed;

        [Header("ジャンプ")] [SerializeField] private float jumpForce;

        [SerializeField] private Vector2 groundCheckOffset;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius;

        public float MoveSpeed => moveSpeed;

        public float JumpForce => jumpForce;
        public Vector2 GroundCheckOffset => groundCheckOffset;
        public LayerMask GroundLayer => groundLayer;
        public float GroundCheckRadius => groundCheckRadius;
    }
}