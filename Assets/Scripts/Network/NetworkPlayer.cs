using Cinemachine;
using Fusion;

namespace GDT.Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;

        private void Awake()
        {
            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        }

        public override void Spawned()
        {
            _virtualCamera.gameObject.SetActive(Object.HasInputAuthority);
        }
    }
}
