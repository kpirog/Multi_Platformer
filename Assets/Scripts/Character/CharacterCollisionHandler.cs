using Fusion;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterCollisionHandler : SimulationBehaviour
    {
        private NetworkRigidbody2D _rb;
        private CharacterAnimationHandler _animationHandler;

        private void Awake()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
            _animationHandler = GetComponent<CharacterAnimationHandler>();
        }
        
        public void PushOff(Vector2 hitPoint, float pushForce)
        {
            Vector2 direction = hitPoint - (Vector2)transform.position;
            direction = -direction.normalized;
            
            _rb.Rigidbody.AddForce(direction * pushForce * Runner.DeltaTime, ForceMode2D.Impulse);
            _animationHandler.SetGetHitAnimation();
        }
    }
}