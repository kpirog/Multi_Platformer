using Fusion;
using UnityEngine;
using Medicine;

namespace GDT.Grappling
{
    public class GrappleHook : NetworkBehaviour
    {
        [SerializeField] private Transform parent;
        [Inject] private LineRenderer LineRenderer { get; }
        [Networked] public GrappleState State { get; private set; } = GrappleState.Disconnected; //jak nie bylo networked to sie psulo na cliencie
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
            Vector2 position = transform.position;
            RPC_SetRopeVisible(position, position, false);
        }

        public void Release()
        {
            State = GrappleState.Released;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_SetRopeVisible(Vector2 startPos, Vector2 endPos, bool visible)
        {
            LineRenderer.enabled = visible;
            LineRenderer.SetPosition(0, startPos);
            LineRenderer.SetPosition(1, endPos);
        }
    }

    public enum GrappleState
    {
        Released,
        Connected,
        Disconnected
    }
}