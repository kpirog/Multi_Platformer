using UnityEngine;

namespace GDT.Character
{
    public class CharacterAnimationHandler : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private const string MoveAnimationKey = "IsMoving";
        private const string JumpAnimationKey = "Jump";
        private const string FallDownAnimationKey = "IsFallingDown";
        
        public void SetSpriteDirection(Vector2 direction)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        public void SetMovementAnimation(bool isMoving)
        {
            animator.SetBool(MoveAnimationKey, isMoving);
        }

        public void SetJumpAnimation()
        {
            animator.SetTrigger(JumpAnimationKey);
        }

        public void SetFallDownAnimation(bool isFallingDown)
        {
            animator.ResetTrigger(JumpAnimationKey);
            animator.SetBool(FallDownAnimationKey, isFallingDown);
        }
    }
}