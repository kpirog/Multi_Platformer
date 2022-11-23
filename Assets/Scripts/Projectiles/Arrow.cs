using Fusion;
using GDT.Character.Effects;
using UnityEngine;
using CharacterController = GDT.Character.CharacterController;
using Medicine;

namespace GDT.Projectiles
{
    public class Arrow : NetworkBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;
        [SerializeField] private float collisionOrigin;
        [SerializeField] private float collisionDistance;

        [SerializeField] private int damage;

        [SerializeField] private LayerMask hitBoxLayer;
        [SerializeField] private LayerMask collisionLayer;

        [SerializeField] private CharacterEffect characterEffect;
        private TickTimer LifeTimer { get; set; }
        [Inject] private NetworkRigidbody2D Rb { get; }
        
        private bool _collisionActive;
        public float Speed => speed;
        
        public override void Spawned()
        {
            LifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
        }

        private void Update()
        {
            if (Rb.Rigidbody.velocity == Vector2.zero) return;
            transform.right = Rb.Rigidbody.velocity;
        }

        public override void FixedUpdateNetwork()
        {
            if (_collisionActive)
            {
                CheckCollision();
            }

            if (LifeTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }

        public void Release(Vector2 direction, float stretchStrength)
        {
            _collisionActive = true;
            Rb.Rigidbody.AddForce(direction * (stretchStrength * speed) , ForceMode2D.Impulse);
        }

        private void CheckCollision()
        {
            if (Runner.LagCompensation.Raycast(transform.position + (transform.right * collisionOrigin),
                    transform.right, collisionDistance, Object.InputAuthority, out var hit, hitBoxLayer,
                    HitOptions.IncludePhysX))
            {
                var character = hit.GameObject.GetComponentInParent<CharacterController>();
                RPC_SetAfterCollision(character);
                characterEffect.ApplyTo(character, hit.Point);
                return;
            }

            var colliderHits = Runner.GetPhysicsScene2D().Raycast(transform.position + (transform.right * collisionOrigin),
                transform.right, collisionDistance, collisionLayer);

            if (colliderHits)
            {
                var damageable = colliderHits.collider.gameObject.GetComponentInParent<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                    LifeTimer = TickTimer.None;
                    Runner.Despawn(Object);
                    return;
                }
                
                RPC_SetAfterCollision(null);
            }
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_SetAfterCollision(CharacterController character)
        {
            if (character)
            {
                transform.SetParent(character.transform);
            }
            
            Rb.Rigidbody.simulated = false;
            _collisionActive = false;
        }
    }
}