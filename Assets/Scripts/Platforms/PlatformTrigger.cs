using Fusion;
using UnityEngine;

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

            if (characterCollider)
            {
                //_platformDurability.DecreaseDurability(Runner.DeltaTime); //different solution
                _platformDurability.StartDestroying();
                return;
            }
            
            _platformDurability.StopDestroying();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube((Vector2)_collider.bounds.center, _collider.bounds.size);
        }
    }
}