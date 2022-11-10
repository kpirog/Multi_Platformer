using UnityEngine;
using NetworkPlayer = GDT.Network.NetworkPlayer;

namespace GDT.Character.Effects
{

    [CreateAssetMenu(menuName = "Character Effects/Slow", fileName = "Slow Effect")]
    public class SlowEffect : CharacterEffect
    {
        [SerializeField] private float slowTime;
        [SerializeField] private float slowMultiplier;
        
        public override void ApplyTo(CharacterController character)
        {
            character.movementHandler.SetSlow(slowTime, slowMultiplier);
        }
    }
}