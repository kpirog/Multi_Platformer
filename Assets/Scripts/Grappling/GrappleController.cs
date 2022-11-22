using Fusion;
using GDT.Data;
using UnityEngine;

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

        private Vector2 _grapplePoint;
        private Vector2 _grappleDirection;
        private Vector2 _targetPosition;

        private bool _hasReachedTarget;
        private bool _isChangingDistance;
        
        private float _ropeLenght;
        
        private NetworkRigidbody2D _rb;
        [Networked] private NetworkButtons PressedButtons { get; set; }
        private bool HasReachedTarget => _targetPosition == _grapplePoint;

        private void Awake()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
        }

        public override void Spawned()
        {
            var interpolationData = Object.HasInputAuthority
                ? InterpolationDataSources.Predicted
                : InterpolationDataSources.Snapshots;
            
            _rb.InterpolationDataSource = interpolationData;
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
                    _targetPosition = Vector2.MoveTowards(_targetPosition, _grapplePoint, Runner.DeltaTime * releaseSpeed);

                    if (Object.HasStateAuthority)
                    {
                        grappleHook.RPC_SetRopeVisible(transform.position, _targetPosition, true);
                    }

                    if (HasReachedTarget)
                    {
                        grappleHook.Connect(_grapplePoint);
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
            if (grappleHook.State != GrappleState.Disconnected)
            {
                grappleHook.Disconnect();
                return;
            }

            Vector2 position = transform.position;
            var mouseHit = Runner.GetPhysicsScene2D().Linecast(position, mousePosition, rayLayerMask);
            
            if (!mouseHit.collider)
            {
                Debug.Log("There is no collider!");
                return;
            }

            if (Vector2.Distance(transform.position, mouseHit.point) < ropeRange.x ||
                Vector2.Distance(transform.position, mouseHit.point) > ropeRange.y)
            {
                return;
            }
            
            _grapplePoint = mouseHit.point;
            _targetPosition = transform.position;
            _ropeLenght = Vector2.Distance(_targetPosition, mouseHit.point);
            grappleHook.Release(mouseHit.transform, mouseHit.point);
        }

        private void Grapple()
        {
            if (_grappleDirection.magnitude < _ropeLenght)
            {
                var perpendicular = Vector2.Perpendicular(_grappleDirection).normalized;
                var newDirection = Vector2.Dot(_rb.Rigidbody.velocity, perpendicular) * perpendicular;

                _rb.Rigidbody.velocity = newDirection.normalized * _rb.Rigidbody.velocity.magnitude;
            }
            else
            {
                _rb.Rigidbody.AddForce(_grappleDirection.normalized * pullForce);
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
            
            _rb.Rigidbody.AddForce(direction * pullForce);
            Debug.Log($"Rope lenght = {_ropeLenght}");
        }
    }
}