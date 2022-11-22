using Fusion;
using UnityEngine;
using Medicine;

namespace GDT.Character
{
    public class CharacterTouchDetector : NetworkBehaviour
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private LayerMask platformLayer;
        [SerializeField] private float groundCheckMultiplier;

        [Inject.FromChildren] private Collider2D Collider { get; }
        
        private LayerMask _groundCollisionLayer;
        private bool _isSlidingLeft, _isSlidingRight;

        public bool IsStayingOnPlatform =>
            IsGrounded && GroundCollider.gameObject.layer == LayerMask.NameToLayer("Platform");
        public Collider2D GroundCollider { get; private set; }
        public bool IsSliding => _isSlidingLeft || _isSlidingRight;
        public bool IsGrounded => GroundCollider && !Physics2D.GetIgnoreCollision(Collider, GroundCollider);

        private void Awake()
        {
            _groundCollisionLayer = groundLayer | platformLayer;
        }

        public override void FixedUpdateNetwork()
        {
            GroundCollider = Runner.GetPhysicsScene2D()
                .OverlapBox(
                    (Vector2)transform.position + Vector2.down * Collider.bounds.extents.y * groundCheckMultiplier,
                    Collider.bounds.size, 0f, _groundCollisionLayer);
            
            _isSlidingLeft = Runner.GetPhysicsScene2D()
                .OverlapCircle((Vector2)transform.position + (Vector2.left * Collider.bounds.extents.x), 0.01f,
                    wallLayer);

            _isSlidingRight = Runner.GetPhysicsScene2D()
                .OverlapCircle((Vector2)transform.position + (Vector2.right * Collider.bounds.extents.x), 0.01f,
                    wallLayer);
        }
    }
}