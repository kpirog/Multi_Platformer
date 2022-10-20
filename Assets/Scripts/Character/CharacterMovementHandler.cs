using Fusion;
using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterMovementHandler : NetworkBehaviour
    {
        [SerializeField] private float speed;

        private NetworkRigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
        }

        public void Move(Vector2 direction)
        {
            direction.Normalize();
            _rb.Rigidbody.velocity += direction * speed * Runner.DeltaTime;
        }

        public void Stop()
        {
            _rb.Rigidbody.velocity = Vector2.zero;
        }
    }
}