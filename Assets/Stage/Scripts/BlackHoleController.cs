using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

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
            LMotion.Create(transform.position.y, transform.position.y - 0.1f, 1f)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToPositionY(transform)
                .AddTo(this);
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
