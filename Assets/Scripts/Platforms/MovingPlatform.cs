using Fusion;
using UnityEditor;
using UnityEngine;

namespace GDT.Platforms
{
    public class MovingPlatform : NetworkBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private SpriteRenderer[] spriteRenderers;

        public Vector3 destination;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;

        private void Awake()
        {
            _startPosition = transform.position;
            _targetPosition = destination;
        }

        public override void FixedUpdateNetwork()
        {
            if (HasReachedTarget(_targetPosition))
            {
                SetTargetPosition();
            }
            
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, movementSpeed * Runner.DeltaTime);
        }

        private bool HasReachedTarget(Vector3 target)
        {
            return transform.position == target;
        }

        private void SetTargetPosition()
        {
            if (transform.position == destination)
            {
                _targetPosition = _startPosition;
            }
            else if (transform.position == _startPosition)
            {
                _targetPosition = destination;
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, destination);
        }
    }
}