using System.Collections;
using Fusion;
using GDT.Character;
using UnityEngine;
using CharacterController = GDT.Character.CharacterController;

namespace GDT.Projectiles
{
    public class IceArrow : Arrow
    {
        [SerializeField] private int freezeTime;
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        protected override void RPC_AdditionalEffect(NetworkObject networkObject, Vector2 point)
        {
            base.RPC_AdditionalEffect(networkObject, point);
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