using Fusion;
using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterMovementHandler : NetworkBehaviour
    {
        [SerializeField] private float acceleration;
        [SerializeField] private float maxVelocity;
        [SerializeField] private float jumpForce;
        [SerializeField] private float drag;

        [SerializeField] private float wallSlidingMultiplier;
        [SerializeField] private float fallMultiplier;
        [SerializeField] private float lowJumpMultiplier;

        [SerializeField] private Vector2 horizontalVelocityReduction;
        [SerializeField] private Vector2 verticalVelocityReduction;

        private NetworkRigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                _rb.InterpolationDataSource = InterpolationDataSources.Predicted;
            }
        }

        public void Move(NetworkInputData input)
        {
            _rb.Rigidbody.drag = 0f;
            
            if (input.GetButton(InputButton.Left))
            {
                if (_rb.Rigidbody.velocity.x > 0f)
                {
                    _rb.Rigidbody.velocity *= Vector2.up;
                }
                
                _rb.Rigidbody.AddForce(Vector2.left * acceleration * Runner.DeltaTime, ForceMode2D.Force);
            }

            if (input.GetButton(InputButton.Right))
            {
                if (_rb.Rigidbody.velocity.x < 0f)
                {
                    _rb.Rigidbody.velocity *= Vector2.up;
                }
                
                _rb.Rigidbody.AddForce(Vector2.right * acceleration * Runner.DeltaTime, ForceMode2D.Force);
            }
        }

        public void Jump(NetworkButtons pressedButtons, CharacterTouchDetector touchDetector)
        {
            if (pressedButtons.IsSet(InputButton.Jump))
            {
                if (touchDetector.IsGrounded)
                {
                    _rb.Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
                else if (touchDetector.IsSlidingLeft)
                {
                    _rb.Rigidbody.AddForce(Vector2.up + (Vector2.right * jumpForce), ForceMode2D.Impulse);
                }
                else if (touchDetector.IsSlidingRight)
                {
                    _rb.Rigidbody.AddForce(Vector2.up + (Vector2.left * jumpForce), ForceMode2D.Impulse);
                }
            }
        }

        public void BetterJumpLogic(NetworkInputData input, CharacterTouchDetector touchDetector)
        {
            if (touchDetector.IsGrounded) return;

            if (IsFallingDown())
            {
                if (touchDetector.IsSliding && input.AxisPressed())
                {
                    _rb.Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (wallSlidingMultiplier - 1) * Runner.DeltaTime;
                }
                else
                {
                    _rb.Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Runner.DeltaTime;
                }
            }
            else if (_rb.Rigidbody.velocity.y > 0f && !input.GetButton(InputButton.Jump))
            {
                _rb.Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Runner.DeltaTime;
            }
        }

        public void LimitSpeed()
        {
            if (Mathf.Abs(_rb.Rigidbody.velocity.x) > maxVelocity)
            {
                _rb.Rigidbody.velocity *= horizontalVelocityReduction;
            }

            if (Mathf.Abs(_rb.Rigidbody.velocity.y) > maxVelocity)
            {
                _rb.Rigidbody.velocity *= verticalVelocityReduction;
            }
        }

        public void SetDrag()
        {
            _rb.Rigidbody.drag = drag;
        }

        public bool IsFallingDown()
        {
            return _rb.Rigidbody.velocity.y < 0f;
        }
    }
}