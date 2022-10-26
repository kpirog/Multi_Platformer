using Fusion;
using UnityEngine;

namespace GDT.Projectiles
{
    public class StandardArrow : Arrow
    {
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        protected override void RPC_AdditionalEffect(NetworkObject networkObject, Vector2 point)
        {
            base.RPC_AdditionalEffect(networkObject, point);
        }
    }
}