using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GDT.Platforms
{
    public class MovingPlatform : NetworkBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float movementRange;

        private float _startXPos;
        private Vector3 _direction;
        
        public override void Spawned()
        {
            _startXPos = transform.position.x;
            _direction = Random.value > 0.5f ? Vector3.right : Vector3.left;
        }

        public override void FixedUpdateNetwork()
        {
            SetMovementDirection();

            transform.position += _direction * movementSpeed * Runner.DeltaTime;
        }

        private void SetMovementDirection()
        {
            if (transform.position.x >= _startXPos + movementRange)
            {
                _direction = Vector3.left;
            }
            else if (transform.position.x <= _startXPos - movementRange)
            {
                _direction = Vector3.right;
            }
        }
    }
}