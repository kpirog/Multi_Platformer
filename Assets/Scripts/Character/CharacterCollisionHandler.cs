using Fusion;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterCollisionHandler : SimulationBehaviour
    {
        private NetworkRigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
        }
        
        public void PushOff(Vector2 hitPoint, float pushForce)
        {
            float horizontalPush = hitPoint.x > transform.position.x ? -1f : 1f;
            float verticalPush = hitPoint.y > transform.position.y ? -1f : 1f;
            Vector2 pushDirection = new Vector2(horizontalPush, verticalPush);

            _rb.Rigidbody.AddForce(pushDirection * pushForce * Runner.DeltaTime, ForceMode2D.Impulse);
        }
    }
}