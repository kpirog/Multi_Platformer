using UnityEngine;

namespace GDT.Character
{
    public class CharacterAnimationHandler : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private const string MoveAnimationKey = "IsMoving";
        
        public void SetSpriteDirection(Vector2 direction)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        public void SetMovementAnimation(bool isMoving)
        {
            animator.SetBool(MoveAnimationKey, isMoving);
        }
    }
}