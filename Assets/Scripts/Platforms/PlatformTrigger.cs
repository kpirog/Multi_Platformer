using Fusion;
using UnityEngine;
using Medicine;

namespace GDT.Platforms
{
    public class PlatformTrigger : NetworkBehaviour
    {
        [SerializeField] private LayerMask collisionLayer;
        [Inject] private Collider2D Collider { get; }
        [Inject] private PlatformDurability PlatformDurability { get; }
        
        public override void FixedUpdateNetwork()
        {
            var characterCollider = Runner.GetPhysicsScene2D().OverlapBox(Collider.bounds.center, Collider.bounds.size, 0f, collisionLayer);

            if (characterCollider)
            {
                //_platformDurability.DecreaseDurability(Runner.DeltaTime); //different solution
                PlatformDurability.StartDestroying();
                return;
            }
            
            PlatformDurability.StopDestroying();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube((Vector2)Collider.bounds.center, Collider.bounds.size);
        }
    }
}