using Fusion;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterCollisionHandler : SimulationBehaviour
    {
        private CharacterAnimationHandler _animationHandler;
        private CharacterTouchDetector _touchDetector;
        
        private NetworkRigidbody2D _rb;
        private Collider2D _collider;
        private Collider2D _platformCollider;

        private void Awake()
        {
            _animationHandler = GetComponent<CharacterAnimationHandler>();
            _touchDetector = GetComponent<CharacterTouchDetector>();
            _rb = GetComponent<NetworkRigidbody2D>();
            _collider = GetComponentInChildren<BoxCollider2D>();
        }

        public override void FixedUpdateNetwork()
        {
            if (_touchDetector.IsGrounded && _platformCollider && Physics2D.GetIgnoreCollision(_collider, _platformCollider))
            {
                IncludePlatformCollider();
            }
            
            if (_touchDetector.IsStayingOnPlatform)
            {
                _platformCollider = _touchDetector.GroundCollider;
            }
        }
        
        public void ExcludePlatformCollider()
        {
            if (!_touchDetector.IsStayingOnPlatform) return;
            Physics2D.IgnoreCollision(_collider, _platformCollider);
        }

        private void IncludePlatformCollider()
        {
            Physics2D.IgnoreCollision(_collider, _platformCollider, false);
            _platformCollider = null;
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