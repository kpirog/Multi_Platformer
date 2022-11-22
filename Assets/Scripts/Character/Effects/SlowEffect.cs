using UnityEngine;

namespace GDT.Character.Effects
{

    [CreateAssetMenu(menuName = "Character Effects/Slow", fileName = "Slow Effect")]
    public class SlowEffect : CharacterEffect
    {
        [SerializeField] private float slowTime;
        [SerializeField][Range(0.01f, 1f)] private float slowMultiplier;
        
        public override void ApplyTo(CharacterController character, Vector2? sourcePosition)
        {
            character.MovementHandler.SetSlow(slowTime, slowMultiplier);
        }
    }
}