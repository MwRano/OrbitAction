using UnityEngine;

namespace Orbit.Player
{
    [CreateAssetMenu(fileName = "PlayerParam", menuName = "Scriptable Objects/PlayerParam")]
    public class PlayerParam : ScriptableObject
    {
        [Header("移動")] [SerializeField] private float moveSpeed;

        [Header("ジャンプ")] [SerializeField] private float jumpForce;

        [Header("Planet引力")]
        [SerializeField] private float planetAttractInitialImpulse = 12f;
        [SerializeField] private float planetAttractForce = 55f;
        [SerializeField] private float planetAttractMaxSpeed = 18f;
        [Range(0f, 1f)]
        [SerializeField] private float planetOrbitGravityScaleRate = 0.3f;
        [Min(0f)]
        [SerializeField] private float planetOrbitInertiaDamping = 8f;

        [SerializeField] private Vector2 groundCheckOffset;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius;

        public float MoveSpeed => moveSpeed;

        public float JumpForce => jumpForce;
        public float PlanetAttractInitialImpulse => planetAttractInitialImpulse;
        public float PlanetAttractForce => planetAttractForce;
        public float PlanetAttractMaxSpeed => planetAttractMaxSpeed;
        public float PlanetOrbitGravityScaleRate => planetOrbitGravityScaleRate;
        public float PlanetOrbitInertiaDamping => planetOrbitInertiaDamping;
        public Vector2 GroundCheckOffset => groundCheckOffset;
        public LayerMask GroundLayer => groundLayer;
        public float GroundCheckRadius => groundCheckRadius;
    }
}
