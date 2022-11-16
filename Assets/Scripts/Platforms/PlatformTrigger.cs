using System;
using Fusion;
using UnityEngine;
using CharacterController = GDT.Character.CharacterController;

namespace GDT.Platforms
{
    public class PlatformTrigger : NetworkBehaviour
    {
        [SerializeField] private LayerMask collisionLayer;
        
        private Collider2D _collider;
        private PlatformDurability _platformDurability;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _platformDurability = GetComponent<PlatformDurability>();
        }

        public override void FixedUpdateNetwork()
        {
            var characterCollider = Runner.GetPhysicsScene2D().OverlapBox(_collider.bounds.center, _collider.bounds.size, 0f, collisionLayer);

            if (characterCollider != null)
            {
                var character = characterCollider.GetComponentInParent<CharacterController>();
                _platformDurability.DecreaseDurability(Runner.DeltaTime);
                Debug.Log($"Player: {character.name} in Trigger!");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube((Vector2)_collider.bounds.center, _collider.bounds.size);
        }
    }
}