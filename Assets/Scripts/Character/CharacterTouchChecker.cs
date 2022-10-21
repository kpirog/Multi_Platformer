using System;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterTouchChecker : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private LayerMask groundCheckLayer;
        [SerializeField] private LayerMask wallTouchLayer;

        [SerializeField] private float wallTouchDistance;
        
        private Bounds _bounds;
        private bool _touchingLeftWall;
        private bool _touchingRightWall;

        public bool IsGrounded { get; private set; }
        public bool IsSliding => (_touchingLeftWall || _touchingRightWall) && !IsGrounded;
        
        private void Awake()
        {
            _bounds = boxCollider.bounds;
        }

        private void Update()
        {
            RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, _bounds.size, 0f, Vector2.down,
                _bounds.extents.y, groundCheckLayer);

            IsGrounded = hit.collider;

            _touchingLeftWall = Physics2D.Raycast(transform.position, -transform.right, wallTouchDistance, wallTouchLayer);
            _touchingRightWall = Physics2D.Raycast(transform.position, transform.right, wallTouchDistance, wallTouchLayer);
        }
    }
}