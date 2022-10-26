using Fusion;
using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterAnimationHandler : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private static readonly int MovementKey = Animator.StringToHash("IsMoving");
        private static readonly int JumpKey = Animator.StringToHash("Jump");
        private static readonly int FallDownKey = Animator.StringToHash("IsFallingDown");
        private static readonly int ShootKey = Animator.StringToHash("IsShooting");
        private static readonly int GetHitKey = Animator.StringToHash("GetHit");

        public void SetSpriteDirection(Vector2 direction)
        {
            spriteRenderer.flipX = direction.x < 0;
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