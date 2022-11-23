using Fusion;
using GDT.Character.Effects;
using Medicine;
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
        
        [Inject.FromChildren] private Collider2D Collider { get; }
        [Inject.FromChildren] private SpriteRenderer SpriteRenderer { get; }
        [Inject] private NetworkRigidbody2D Rb { get; }

        
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
                    characterEffect.ApplyTo(player, transform.position);
                }
            }

            Exploded = true;
            Invoke(nameof(DespawnBarrel), particles.main.duration);
        }
        
        private static void OnExploded(Changed<Barrel> changed)
        {
            changed.Behaviour.DisableComponents();
            changed.Behaviour.particles.Play();
        }
        
        private void DespawnBarrel()
        {
            Runner.Despawn(Object);
        }

        private void DisableComponents()
        {
            Collider.enabled = false;
            SpriteRenderer.enabled = false;
            Rb.Rigidbody.simulated = false;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, explosionRange);
        }
    }
}