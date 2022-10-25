using System;
using Fusion;
using UnityEngine;

namespace Projectiles
{
    public abstract class Arrow : NetworkBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float collisionOrigin;
        [SerializeField] private float collisionDistance;
        
        [SerializeField] private LayerMask collisionLayer;
        
        private bool _collisionActive;
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

        public override void FixedUpdateNetwork()
        {
            if (_collisionActive)
            {
                CheckCollision();
            }
        }

        public void Release(float angle, float stretchForce)
        {
            var direction = Quaternion.Euler(0f, 0f, angle) * transform.right;
            Rb.Rigidbody.AddForce(direction * (stretchForce * speed) * Runner.DeltaTime, ForceMode2D.Impulse);
            _collisionActive = true;
        }

        private void CheckCollision()
        {
            if (Runner.LagCompensation.Raycast(transform.position + (transform.right * collisionOrigin),
                    transform.right, collisionDistance, Object.InputAuthority, out var hit, collisionLayer, HitOptions.IncludePhysX))
            {
                RPC_SetAfterCollision(hit.GameObject.GetComponentInParent<NetworkObject>());
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_SetAfterCollision(NetworkObject networkObject)
        {
            transform.SetParent(networkObject.transform);
            Rb.Rigidbody.simulated = false;
            _collisionActive = false;
        }
    }
}