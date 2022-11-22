using Fusion;
using UnityEngine;
using Medicine;

namespace GDT.Character
{
    public class CharacterCollisionHandler : SimulationBehaviour
    {
        [Inject] private CharacterAnimationHandler AnimationHandler { get; }
        [Inject] private CharacterTouchDetector TouchDetector { get; }
        [Inject] private NetworkRigidbody2D Rb { get; }
        [Inject.FromChildren] private Collider2D Collider { get; }
        
        private Collider2D _platformCollider;
        
        public override void FixedUpdateNetwork()
        {
            if (TouchDetector.IsGrounded && _platformCollider && Physics2D.GetIgnoreCollision(Collider, _platformCollider))
            {
                IncludePlatformCollider();
            }
            
            if (TouchDetector.IsStayingOnPlatform)
            {
                _platformCollider = TouchDetector.GroundCollider;
            }
        }
        
        public void ExcludePlatformCollider()
        {
            if (!TouchDetector.IsStayingOnPlatform) return;
            Physics2D.IgnoreCollision(Collider, _platformCollider);
        }

        private void IncludePlatformCollider()
        {
            Physics2D.IgnoreCollision(Collider, _platformCollider, false);
            _platformCollider = null;
        }
        
        public void Push(Vector2 hitPoint, float pushForce)
        {
            var direction = hitPoint - (Vector2)transform.position;
            direction = -direction.normalized;
            
            Rb.Rigidbody.AddForce(direction * pushForce * Runner.DeltaTime, ForceMode2D.Impulse);
            AnimationHandler.SetGetHitAnimation();
        }
        
        public void ExplosionPush(Vector2 hitPoint, float pushForce)
        {
            var characterPosition = (Vector2)transform.position;
            var distance = Vector2.Distance(hitPoint, characterPosition);
            
            Debug.Log($"Explosion distance = {distance}");
            
            var direction = hitPoint - characterPosition;
            direction = -direction.normalized;
            
            Rb.Rigidbody.AddForce(direction * pushForce * distance * Runner.DeltaTime, ForceMode2D.Impulse);
            AnimationHandler.SetGetHitAnimation();
        }
    }
}