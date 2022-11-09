using Fusion;
using UnityEngine;
using CharacterController = GDT.Character.CharacterController;

namespace GDT.Projectiles
{
    public class IceArrow : Arrow
    {
        [SerializeField] private int freezeTime;

        protected override void AdditionalEffect(NetworkObject networkObject, Vector2 point)
        {
            base.AdditionalEffect(networkObject, point);
            FreezePlayer(networkObject);
        }

        private void FreezePlayer(NetworkObject networkObject)
        {
            var characterController = networkObject.GetComponent<CharacterController>();
            Debug.Log("Freeze effect");
            characterController.inputHandler.FreezeInputForSeconds(freezeTime);
        }
    }
}