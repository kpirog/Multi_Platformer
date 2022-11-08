using Cinemachine;
using Fusion;
using GDT.Data;


namespace GDT.Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        public static NetworkPlayer Local { get; private set; }
        
        [Networked] public NetworkString<_16> NickName { get; private set; }
        [Networked] public bool Ready { get; private set; }
        
        private CinemachineVirtualCamera _virtualCamera;
        
        private void Awake()
        {
            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            DontDestroyOnLoad(gameObject);
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
                RPC_SetNickname(LocalPlayerDataHandler.Nickname);
            }

            _virtualCamera.gameObject.SetActive(Object.HasInputAuthority);
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
