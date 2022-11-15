using GDT.Character.Effects;
using UnityEngine;
using CharacterController = GDT.Character.CharacterController;

namespace Character.Effects
{
    [CreateAssetMenu(menuName = "Character Effects/Push", fileName = "Push Effect")]
    public class PushEffect : CharacterEffect
    {
        [SerializeField] private float force;
        public override void ApplyTo(CharacterController character, Vector2? sourcePosition)
        {
            character.collisionHandler.Push(sourcePosition ?? character.transform.position, force);
        }
    }
}
