using UnityEngine;

namespace GDT.Character.Effects
{
    [CreateAssetMenu(menuName = "Character Effects/Explosion Push", fileName = "Explosion Push Effect")]
    public class ExplosionPushEffect : CharacterEffect
    {
        [SerializeField] private float force;
        public override void ApplyTo(CharacterController character, Vector2? sourcePosition)
        {
            character.collisionHandler.ExplosionPush(sourcePosition ?? character.transform.position, force);
        }
    }
}