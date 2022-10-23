using Fusion;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterTouchDetector : NetworkBehaviour
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private float groundCheckMultiplier;

        private Collider2D _collider;
        
        public bool IsGrounded { get; private set; }
        public bool IsSlidingLeft { get; private set; }
        public bool IsSlidingRight { get; private set; }
        
        public bool IsSliding => IsSlidingLeft || IsSlidingRight;

        private void Awake()
        {
            _collider = GetComponentInChildren<BoxCollider2D>();
        }

        public override void FixedUpdateNetwork()
        {
            IsGrounded = Runner.GetPhysicsScene2D()
                .OverlapBox(
                    (Vector2)transform.position + Vector2.down * _collider.bounds.extents.y * groundCheckMultiplier,
                    _collider.bounds.size, 0f, groundLayer);

            IsSlidingLeft = Runner.GetPhysicsScene2D()
                .OverlapCircle((Vector2)transform.position + (Vector2.left * _collider.bounds.extents.x), 0.01f,
                    wallLayer);

            IsSlidingRight = Runner.GetPhysicsScene2D()
                .OverlapCircle((Vector2)transform.position + (Vector2.right * _collider.bounds.extents.x), 0.01f,
                    wallLayer);
        }
    }
}