using Fusion;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterGrapplingHook : NetworkBehaviour
    {
        [SerializeField] private DistanceJoint2D distanceJoint;

        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            _lineRenderer.enabled = false;
            distanceJoint.enabled = false;
        }

        public void Release(RaycastHit2D mouseHit)
        {
            //Detach();
            
            if (!mouseHit.collider)
            {
                Debug.Log("There is no collider!");
                return;
            }

            Vector2 connectPoint;
            var networkRb = mouseHit.transform.GetComponent<NetworkRigidbody2D>();

            if (networkRb)
            {
                distanceJoint.connectedBody = networkRb.Rigidbody;
                connectPoint = mouseHit.point - (Vector2)mouseHit.transform.position;
            }
            else
            {
                connectPoint = mouseHit.point;
            }
            
            RPC_ConnectRope(connectPoint, mouseHit.point);
        }
        
        public void DetachRope()
        {
            distanceJoint.enabled = false;
            distanceJoint.connectedBody = null;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_ConnectRope(Vector2 connectPoint, Vector2 hitPoint)
        {
            distanceJoint.enabled = true;
            distanceJoint.distance = Vector2.Distance(transform.position, hitPoint);
            distanceJoint.connectedAnchor = connectPoint;
        }
    }
}