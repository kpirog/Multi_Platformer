using Fusion;
using UnityEngine;
using CharacterController = GDT.Character.CharacterController;

namespace GDT.Platforms
{
    public class PlatformTrigger : NetworkBehaviour
    {
        [SerializeField] private Collider2D _collider;
        [SerializeField] private LayerMask collisionLayer;
        [SerializeField] private Vector2 offset;

        public override void Spawned()
        {
            _collider = GetComponent<Collider2D>();
            Debug.Log("Spawned");
        }

        public override void FixedUpdateNetwork()
        {
            Debug.Log("Fixed Update Network");
            
            var characterCollider = Runner.GetPhysicsScene2D().OverlapBox((Vector2)_collider.bounds.center + offset, _collider.bounds.size, 0f, collisionLayer);
            
            Debug.Log(characterCollider);
            
            if (characterCollider != null)
            {
                var character = characterCollider.GetComponentInParent<CharacterController>();
                Debug.Log($"Player: {character.name} in Trigger!");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube((Vector2)_collider.bounds.center + offset, _collider.bounds.size);
        }
    }
}