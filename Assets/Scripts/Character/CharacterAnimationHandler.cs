using Fusion;
using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterAnimationHandler : NetworkBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private static readonly int MovementKey = Animator.StringToHash("IsMoving");
        private static readonly int JumpKey = Animator.StringToHash("Jump");
        private static readonly int FallDownKey = Animator.StringToHash("IsFallingDown");
        private static readonly int ShootKey = Animator.StringToHash("IsShooting");
        private static readonly int GetHitKey = Animator.StringToHash("GetHit");

        [Networked(OnChanged = nameof(OnSpriteDirectionChanged))] public Vector2 SpriteDirection { get; set; }

        private static void OnSpriteDirectionChanged(Changed<CharacterAnimationHandler> changed)
        {
            changed.Behaviour.spriteRenderer.flipX = changed.Behaviour.SpriteDirection.x < 0;
        }
        
        public void SetMovementAnimation(bool isMoving)
        {
            animator.SetBool(MovementKey, isMoving);
        }

        public void SetJumpAnimation(NetworkButtons pressedButtons)
        {
            if (pressedButtons.IsSet(InputButton.Jump))
            {
                animator.SetTrigger(JumpKey);
            }
        }

        public void SetFallDownAnimation(bool isFallingDown)
        {
            animator.ResetTrigger(JumpKey);
             animator.SetBool(FallDownKey, isFallingDown);
        }

        public void SetShootAnimation()
        {
            animator.SetBool(ShootKey, true);
        }

        public void StopShootAnimation()
        {
            animator.SetBool(ShootKey, false);
        }

        public void SetGetHitAnimation()
        {
            animator.SetTrigger(GetHitKey);
        }
    }
}