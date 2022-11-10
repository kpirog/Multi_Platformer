using UnityEngine;

namespace GDT.Character.Effects
{
    public abstract class CharacterEffect : ScriptableObject
    {
        public abstract void ApplyTo(CharacterController character);
    }
}