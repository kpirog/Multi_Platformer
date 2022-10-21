using UnityEngine;

namespace GDT.Character
{
    public class CharacterGroundChecker : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private LayerMask groundCheckLayer;

        private Bounds _bounds;
        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _bounds = boxCollider.bounds;
        }

        private void Update()
        {
            RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, _bounds.size, 0f, Vector2.down,
                _bounds.extents.y, groundCheckLayer);

            IsGrounded = hit.collider;
        }
    }
}