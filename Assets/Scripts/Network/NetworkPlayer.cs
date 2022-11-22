using Cinemachine;
using Fusion;
using GDT.Data;
using Medicine;


namespace GDT.Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [Networked] public NetworkString<_16> NickName { get; private set; }
        [Networked] public bool Ready { get; private set; }
        [Inject.FromChildren] private CinemachineVirtualCamera VirtualCamera { get; }
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                RPC_SetNickname(LocalPlayerDataHandler.Nickname);
            }

            VirtualCamera.gameObject.SetActive(Object.HasInputAuthority);
            PlayerManager.RegisterPlayer(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            PlayerManager.UnregisterPlayer(this);
        }
        
        public void ToggleReady()
        {
            Ready = !Ready;
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
