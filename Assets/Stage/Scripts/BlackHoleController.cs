using UnityEngine;
using Utility;

namespace Stage
{
    public class BlackHoleController : MonoBehaviour
    {
        [Header("Attractor Settings")]
        [SerializeField] float radius = 5f;
        [SerializeField] float force = 20f;
        
        private readonly Collider2D[] _results = new Collider2D[2];

        private void Awake()
        {
            MotionCreator.CreateFloatingMotion(transform, 0.1f, 1f);
        }
        
        private void FixedUpdate()
        {
            Suck();
        }

        private void Suck()
        {
            Vector2 center = transform.position;
            var filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Player"));
            filter.useTriggers = true;
            int count = Physics2D.OverlapCircle(center, radius, filter, _results);
            if (count != 1) return; 
            
            var col = _results[0];
            var rb = col.attachedRigidbody;

            Vector2 dir = center - rb.position;
            float dist = dir.sqrMagnitude;
            if (dist <= 0.01f) return;
            
            rb.AddForce(dir.normalized * force, ForceMode2D.Force);
            
        }
    }
}
