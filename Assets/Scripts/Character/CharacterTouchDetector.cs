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
        private bool _isSlidingLeft, _isSlidingRight;
        
        public bool IsSliding => _isSlidingLeft || _isSlidingRight;
        public bool IsGrounded => GroundCollider;

        public Collider2D GroundCollider { get; private set; }

        private void Awake()
        {
            _collider = GetComponentInChildren<BoxCollider2D>();
        }

        public override void FixedUpdateNetwork()
        {
            GroundCollider = Runner.GetPhysicsScene2D()
                .OverlapBox(
                    (Vector2)transform.position + Vector2.down * _collider.bounds.extents.y * groundCheckMultiplier,
                    _collider.bounds.size, 0f, groundLayer);

            _isSlidingLeft = Runner.GetPhysicsScene2D()
                .OverlapCircle((Vector2)transform.position + (Vector2.left * _collider.bounds.extents.x), 0.01f,
                    wallLayer);

            _isSlidingRight = Runner.GetPhysicsScene2D()
                .OverlapCircle((Vector2)transform.position + (Vector2.right * _collider.bounds.extents.x), 0.01f,
                    wallLayer);
        }
    }
}