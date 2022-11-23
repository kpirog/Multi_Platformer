using Fusion;
using UnityEngine;
using Medicine;

namespace GDT.Grappling
{
    public class GrappleHook : NetworkBehaviour
    {
        [SerializeField] private Transform parent;
        [Inject] private LineRenderer LineRenderer { get; }
        [Networked] public GrappleState State { get; private set; } = GrappleState.Disconnected; 
        public Vector2 Position => transform.GetChild(0).position;
        
        public void Connect(Transform connectedTransform, Vector2 connectPoint)
        {
            transform.position = connectPoint;
            transform.SetParent(connectedTransform);
            State = GrappleState.Connected;
        }

        public void Disconnect()
        {
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            State = GrappleState.Disconnected;
            if (!Object.HasStateAuthority) return;
            RPC_HideRope();
        }

        public void Release()
        {
            State = GrappleState.Released;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_DrawRope(Vector2 startPos, Vector2 endPos)
        {
            LineRenderer.enabled = true;
            LineRenderer.SetPosition(0, startPos);
            LineRenderer.SetPosition(1, endPos);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_HideRope()
        {
            LineRenderer.enabled = false;
        }
    }

    public enum GrappleState
    {
        Released,
        Connected,
        Disconnected
    }
}