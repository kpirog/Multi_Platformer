using Cinemachine;
using Fusion;
using GDT.Data;
using UnityEngine;
using CharacterController = GDT.Character.CharacterController;

namespace GDT.Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        private CharacterController _characterController;

        [Networked(nameof(OnStateChanged))] 
        private PlayerState State { get; set; } = PlayerState.Inactive;
        
        public static NetworkPlayer Local { get; private set; }
        
        [Networked] public NetworkString<_16> NickName { get; set; }
        
        public enum PlayerState
        {
            Inactive,
            Active,
            Dead
        }
        
        private void Awake()
        {
            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            _characterController = GetComponent<CharacterController>();
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
                RPC_SetNickname(LocalPlayerDataHandler.Nickname);
            }

            _virtualCamera.gameObject.SetActive(Object.HasInputAuthority);
            _characterController.SetMovementEnabled(false);
            _characterController.SetModelVisible(false);
            PlayerManager.RegisterPlayer(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            PlayerManager.UnregisterPlayer(this);
        }

        private static void OnStateChanged(Changed<NetworkPlayer> changed)
        {

        }
        
        public float GetCurrentHeight()
        {
            return transform.position.y;
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_SetNickname(string nickname)
        {
            NickName = nickname;
        }
    }
}
