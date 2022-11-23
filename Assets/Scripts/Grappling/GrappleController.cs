using Fusion;
using GDT.Data;
using UnityEngine;
using Medicine;

namespace GDT.Grappling
{
    [RequireComponent(typeof(NetworkRigidbody2D))]
    public class GrappleController : NetworkBehaviour
    {
        [SerializeField] private float pullForce;
        [SerializeField] private float releaseSpeed;
        [SerializeField] private Vector2 ropeRange;

        [SerializeField] private GrappleHook grappleHook;
        [SerializeField] private LayerMask rayLayerMask;

        private Vector2 _grappleDirection;
        private Vector2 _mousePosition;
        private Vector2 _ropePosition;
        private Vector2 _contactPoint;

        private RaycastHit2D _raycastHit;
        private float _ropeLenght;
        
        private bool _isChangingDistance;
        private bool HasReachedTarget => _ropePosition == _contactPoint;
        
        [Inject] private NetworkRigidbody2D Rb { get; }
        [Networked] private NetworkButtons PressedButtons { get; set; }
        
        public override void Spawned()
        {
            var interpolationData = Object.HasInputAuthority
                ? InterpolationDataSources.Predicted
                : InterpolationDataSources.Snapshots;
            
            Rb.InterpolationDataSource = interpolationData;
            InterpolationDataSource = interpolationData;
        }

        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out NetworkInputData input)) return;

            var pressedButtons = input.Buttons.GetPressed(PressedButtons);
            PressedButtons = input.Buttons;

            if (pressedButtons.IsSet(InputButton.GrapplingHook))
            {
                UseHook(input.MousePosition);
            }

            switch (grappleHook.State)
            {
                default:
                case GrappleState.Disconnected:
                {
                    break;
                }
                case GrappleState.Released:
                {
                    Vector2 position = transform.position;

                    if (!HasReachedTarget)
                    {
                        if (_raycastHit.collider)
                        {
                            _contactPoint = _raycastHit.point;
                        }
                        
                        if (Object.HasStateAuthority)
                        {
                            grappleHook.RPC_SetRopeVisible(position, _ropePosition, true);
                        }

                        _ropePosition = Vector2.MoveTowards(_ropePosition, _contactPoint, Runner.DeltaTime * releaseSpeed);
                        _raycastHit = Runner.GetPhysicsScene2D().Linecast(position, _mousePosition, rayLayerMask);
                    }
                    else
                    {
                        if (_raycastHit.collider)
                        {
                            _ropeLenght = Vector2.Distance(position, _ropePosition);
                            grappleHook.Connect(_raycastHit.transform, _ropePosition);
                        }
                        else
                        {
                            grappleHook.Disconnect();
                        }
                    }

                    break;
                }
                case GrappleState.Connected:
                {
                    _grappleDirection = grappleHook.Position - (Vector2)transform.position;
                    HandleRopeDistance(input);
                    
                    if (Object.HasStateAuthority)
                    {
                        grappleHook.RPC_SetRopeVisible(transform.position, grappleHook.Position, true);
                    }
                    
                    if (!_isChangingDistance)
                    {
                        Grapple();
                    }

                    break;
                }
            }
        }

        private void UseHook(Vector2 mousePosition)
        {
            _ropePosition = transform.position;
            _mousePosition = mousePosition;
            _contactPoint = mousePosition;
            _raycastHit = new RaycastHit2D();

            if (grappleHook.State != GrappleState.Disconnected)
            {
                grappleHook.Disconnect();
                return;
            }

            if (Vector2.Distance(transform.position, mousePosition) > ropeRange.y)
            {
                return;
            }

            grappleHook.Release();
        }

        private void Grapple()
        {
            if (_grappleDirection.magnitude < _ropeLenght)
            {
                var perpendicular = Vector2.Perpendicular(_grappleDirection).normalized;
                var newDirection = Vector2.Dot(Rb.Rigidbody.velocity, perpendicular) * perpendicular;

                Rb.Rigidbody.velocity = newDirection.normalized * Rb.Rigidbody.velocity.magnitude;
            }
            else
            {
                Rb.Rigidbody.AddForce(_grappleDirection.normalized * pullForce);
            }
        }

        private void HandleRopeDistance(NetworkInputData input)
        {
            if (input.GetButton(InputButton.DecreaseRope) && _ropeLenght > ropeRange.x)
            {
                ChangeRopeLenght(false);
            }
            else if (input.GetButton(InputButton.IncreaseRope) && _ropeLenght < ropeRange.y)
            {
                ChangeRopeLenght(true);
            }
            else
            {
                _isChangingDistance = false;
            }
        }
        
        private void ChangeRopeLenght(bool increase)
        {
            _isChangingDistance = true;
            _ropeLenght = Vector2.Distance(transform.position, grappleHook.Position);
            
            Vector2 direction = increase
                ? (Quaternion.Euler(0f, 0f, 180f) * _grappleDirection.normalized)
                : _grappleDirection.normalized;
            
            Rb.Rigidbody.AddForce(direction * pullForce);
        }
    }
}