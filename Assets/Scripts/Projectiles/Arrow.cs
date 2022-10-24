using Fusion;
using UnityEngine;

namespace Projectiles
{
    public abstract class Arrow : NetworkBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;

        protected NetworkRigidbody2D Rb;
        
        public void Awake()
        {
            Rb = GetComponent<NetworkRigidbody2D>();
        }
        
        private void Update()
        {
            if (Rb.Rigidbody.velocity == Vector2.zero) return;

            transform.right = Rb.Rigidbody.velocity;
        }

        public void Release(float angle, float stretchForce)
        {
            var direction = Quaternion.Euler(0f, 0f, angle) * transform.right;

            Rb.Rigidbody.AddForce(direction * (stretchForce * speed) * Runner.DeltaTime, ForceMode2D.Impulse);
        }
    }
}