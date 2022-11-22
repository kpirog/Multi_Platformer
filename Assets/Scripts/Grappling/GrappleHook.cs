using Fusion;
using UnityEngine;

namespace GDT.Grappling
{
    public class GrappleHook : NetworkBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private Transform child;
        [SerializeField] private LineRenderer lineRenderer;

        private Transform _connectedTransform;
        [Networked] public GrappleState State { get; private set; } = GrappleState.Disconnected; //jak nie bylo networked to sie psulo na cliencie
        public Vector2 Position => child.position;
        
        public void Connect(Vector2 mousePosition)
        {
            transform.position = mousePosition;
            transform.SetParent(_connectedTransform);
            State = GrappleState.Connected;
        }

        public void Disconnect()
        {
            transform.SetParent(parent);
            _connectedTransform = null;
            State = GrappleState.Disconnected;
            
            if (!Object.HasStateAuthority) return;
            Vector2 position = transform.position;
            RPC_SetRopeVisible(position, position, false);
        }

        public void Release(Transform connectedTransform, Vector2 mousePosition)
        {
            _connectedTransform = connectedTransform;
            State = GrappleState.Released;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_SetRopeVisible(Vector2 startPos, Vector2 endPos, bool visible)
        {
            lineRenderer.enabled = visible;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }

    public enum GrappleState
    {
        Released,
        Connected,
        Disconnected
    }
}