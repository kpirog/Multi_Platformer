using System.Collections;
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
            StartCoroutine(FreezePlayer(networkObject));
        }

        private IEnumerator FreezePlayer(NetworkObject networkObject)
        {
            var characterController = networkObject.GetComponent<CharacterController>();
            Debug.Log("Freeze effect");
            yield return characterController.inputHandler.TurnOffInputForSeconds(freezeTime);
        }
    }
}