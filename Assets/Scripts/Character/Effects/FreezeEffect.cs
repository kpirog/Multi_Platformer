using UnityEngine;

namespace GDT.Character.Effects
{

    [CreateAssetMenu(menuName = "Character Effects/Freeze", fileName = "Freeze Effect")]
    public class FreezeEffect : CharacterEffect
    {
        [SerializeField] private int freezeTime;
        
        public override void ApplyTo(CharacterController character, Vector2? sourcePosition)
        {
            character.inputHandler.FreezeInputForSeconds(freezeTime);
        }
    }
}