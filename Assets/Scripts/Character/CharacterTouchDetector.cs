using Fusion;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterTouchDetector : NetworkBehaviour
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private LayerMask platformLayer;
        [SerializeField] private float groundCheckMultiplier;

        private Collider2D _collider;
        private LayerMask _groundCollisionLayer;
        private bool _isSlidingLeft, _isSlidingRight;

        public bool IsStayingOnPlatform =>
            IsGrounded && GroundCollider.gameObject.layer == LayerMask.NameToLayer("Platform");
        public Collider2D GroundCollider { get; private set; }
        public bool IsSliding => _isSlidingLeft || _isSlidingRight;
        public bool IsGrounded => GroundCollider && !Physics2D.GetIgnoreCollision(_collider, GroundCollider);

        private void Awake()
        {
            _collider = GetComponentInChildren<BoxCollider2D>();
            _groundCollisionLayer = groundLayer | platformLayer;
        }

        public override void FixedUpdateNetwork()
        {
            GroundCollider = Runner.GetPhysicsScene2D()
                .OverlapBox(
                    (Vector2)transform.position + Vector2.down * _collider.bounds.extents.y * groundCheckMultiplier,
                    _collider.bounds.size, 0f, _groundCollisionLayer);
            
            _isSlidingLeft = Runner.GetPhysicsScene2D()
                .OverlapCircle((Vector2)transform.position + (Vector2.left * _collider.bounds.extents.x), 0.01f,
                    wallLayer);

            _isSlidingRight = Runner.GetPhysicsScene2D()
                .OverlapCircle((Vector2)transform.position + (Vector2.right * _collider.bounds.extents.x), 0.01f,
                    wallLayer);
        }
    }
}