using UnityEngine;

namespace GDT.Character.Effects
{
    [CreateAssetMenu(menuName = "Character Effects/Push", fileName = "Push Effect")]
    public class PushEffect : CharacterEffect
    {
        [SerializeField] private float force;
        
        public override void ApplyTo(CharacterController character, Vector2? sourcePosition)
        {
            character.collisionHandler.PushPlayerInOppositeDirection(sourcePosition ?? character.transform.position, force);
        }
    }
}