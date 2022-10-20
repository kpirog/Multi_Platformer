using Fusion;

namespace GDT.Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        public static NetworkPlayer Local { get; private set; }
        
        public NetworkObject networkObject;
        
        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
            }
        }
    }
}