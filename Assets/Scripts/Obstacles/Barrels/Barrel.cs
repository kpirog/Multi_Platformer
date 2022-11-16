using Fusion;
using GDT.Character.Effects;
using UnityEngine;
using CharacterController = GDT.Character.CharacterController;

namespace GDT.Obstacles.Barrels
{
    public class Barrel : NetworkBehaviour, IDamageable
    {
        [SerializeField] private float explosionRange;
        [SerializeField] private LayerMask explosionLayer;
        
        [SerializeField] private CharacterEffect characterEffect;
        [SerializeField] private ParticleSystem particles;

        [Networked(OnChanged = nameof(OnExploded))]
        private NetworkBool Exploded { get; set; }
        
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;
        private NetworkRigidbody2D _rb;

        private void Awake()
        {
            _collider = GetComponentInChildren<Collider2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _rb = GetComponent<NetworkRigidbody2D>();
        }

        public void TakeDamage(int damage)
        {
            Explode();
        }

        private void Explode()
        {
            var collider = Runner.GetPhysicsScene2D().OverlapCircle(transform.position, explosionRange, explosionLayer);

            if (collider != null)
            {
                var player = collider.gameObject.GetComponentInParent<CharacterController>();

                if (player)
                {
                    Debug.Log($"Collision with = {player.gameObject.name}");
                    characterEffect.ApplyTo(player, transform.position);
                }
            }

            Exploded = true;
            Invoke(nameof(DespawnBarrel), particles.main.duration);
        }
        
        private static void OnExploded(Changed<Barrel> changed)
        {
            Debug.Log($"Exploded");
            changed.Behaviour.DisableComponents();
            changed.Behaviour.particles.Play();
        }
        
        private void DespawnBarrel()
        {
            Runner.Despawn(Object);
        }

        private void DisableComponents()
        {
            _collider.enabled = false;
            _spriteRenderer.enabled = false;
            _rb.Rigidbody.simulated = false;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, explosionRange);
        }
    }
}