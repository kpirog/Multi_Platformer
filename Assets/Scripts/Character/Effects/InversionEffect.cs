using UnityEngine;

namespace GDT.Character.Effects
{
    [CreateAssetMenu(menuName = "Character Effects/Inversion", fileName = "Inversion Effect")]
    public class InversionEffect : CharacterEffect
    {
        [SerializeField] private float duration;
        
        public override void ApplyTo(CharacterController character, Vector2? sourcePosition)
        {
            character.InputHandler.InvertControlForSeconds(duration);
        }
    }
}
