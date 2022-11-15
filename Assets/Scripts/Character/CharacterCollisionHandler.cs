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
        
        public void Push(Vector2 hitPoint, float pushForce)
        {
            var direction = hitPoint - (Vector2)transform.position;
            direction = -direction.normalized;
            
            _rb.Rigidbody.AddForce(direction * pushForce * Runner.DeltaTime, ForceMode2D.Impulse);
            _animationHandler.SetGetHitAnimation();
        }
        
        public void ExplosionPush(Vector2 hitPoint, float pushForce)
        {
            var characterPosition = (Vector2)transform.position;
            var distance = Vector2.Distance(hitPoint, characterPosition);
            
            Debug.Log($"Explosion distance = {distance}");
            
            var direction = hitPoint - characterPosition;
            direction = -direction.normalized;
            
            _rb.Rigidbody.AddForce(direction * pushForce * distance * Runner.DeltaTime, ForceMode2D.Impulse);
            _animationHandler.SetGetHitAnimation();
        }
    }
}