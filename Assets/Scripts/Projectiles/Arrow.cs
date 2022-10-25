using Fusion;
using GDT.Character;
using UnityEngine;

namespace Projectiles
{
    public abstract class Arrow : NetworkBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;
        [SerializeField] private float pushMultiplier;
        [SerializeField] private float collisionOrigin;
        [SerializeField] private float collisionDistance;
        [SerializeField] private LayerMask hitBoxLayer;
        [SerializeField] private LayerMask collisionLayer;

        [Networked] private TickTimer LifeTimer { get; set; }

        private NetworkRigidbody2D _rb;
        private bool _collisionActive;
        private float _stretchStrength;
        private float PushForce => _stretchStrength * pushMultiplier;

        private void Awake()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
        }

        public override void Spawned()
        {
            LifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
        }

        private void Update()
        {
            if (_rb.Rigidbody.velocity == Vector2.zero) return;
            transform.right = _rb.Rigidbody.velocity;
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

        public void Release(float angle, float stretchStrength)
        {
            _stretchStrength = stretchStrength;
            _collisionActive = true;
            var direction = Quaternion.Euler(0f, 0f, angle) * transform.right;
            _rb.Rigidbody.AddForce(direction * (stretchStrength * speed) * Runner.DeltaTime, ForceMode2D.Impulse);
        }

        private void CheckCollision()
        {
            if (Runner.LagCompensation.Raycast(transform.position + (transform.right * collisionOrigin),
                    transform.right, collisionDistance, Object.InputAuthority, out var hit, hitBoxLayer,
                    HitOptions.IncludePhysX))
            {
                var networkObject = hit.GameObject.GetComponentInParent<NetworkObject>();
                RPC_SetAfterCollision(networkObject);
                RPC_PushPlayer(networkObject, hit.Point);
                return;
            }

            var colliderHits = Runner.GetPhysicsScene2D().Raycast(transform.position + (transform.right * collisionOrigin),
                transform.right, collisionDistance, collisionLayer);

            if (colliderHits)
            {
                RPC_SetAfterCollision(null);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_SetAfterCollision(NetworkObject networkObject)
        {
            if (networkObject)
            {
                transform.SetParent(networkObject.transform);
            }
            
            _rb.Rigidbody.simulated = false;
            _collisionActive = false;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_PushPlayer(NetworkObject networkObject, Vector2 point)
        {
            networkObject.GetComponent<CharacterCollisionHandler>().PushOff(point, PushForce);
        }
    }
}