using Medicine;
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

        [Inject] private NetworkRigidbody2D Rb { get; }
        [Inject] private CharacterAnimationHandler AnimationHandler { get; }
        
        [Inject.FromChildren] private Collider2D Collider { get; }
        [Networked] private NetworkBool DoubleJump { get; set; }
        [Networked] private TickTimer SlowTimer { get; set; }
        
        private bool _canJumpAgain;
        private float _currentAcceleration;
        private int _slowMultiplier;

        
        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Rb.InterpolationDataSource = InterpolationDataSources.Predicted;
            }
        }
        
        public override void FixedUpdateNetwork()
        {
            if (SlowTimer.ExpiredOrNotRunning(Runner))
            {
                _currentAcceleration = acceleration;
            }
        }

        public void Move(NetworkInputData input)
        {
            Rb.Rigidbody.drag = 0f;
            AnimationHandler.SetMovementAnimation(true);

            if (input.GetButton(InputButton.Left))
            {
                if (Rb.Rigidbody.velocity.x > 0f)
                {
                    Rb.Rigidbody.velocity *= Vector2.up;
                }

                Rb.Rigidbody.AddForce(Vector2.left * _currentAcceleration * Runner.DeltaTime, ForceMode2D.Force);
                AnimationHandler.SetSpriteDirection(Vector2.left);
            }

            if (input.GetButton(InputButton.Right))
            {
                if (Rb.Rigidbody.velocity.x < 0f)
                {
                    Rb.Rigidbody.velocity *= Vector2.up;
                }

                Rb.Rigidbody.AddForce(Vector2.right * _currentAcceleration * Runner.DeltaTime, ForceMode2D.Force);
                AnimationHandler.SetSpriteDirection(Vector2.right);
            }
        }

        public void Jump(NetworkButtons pressedButtons, CharacterTouchDetector touchDetector)
        {
            _canJumpAgain = DoubleJump && !touchDetector.IsGrounded;
            
            if (pressedButtons.IsSet(InputButton.Jump))
            {
                if ((touchDetector.IsGrounded || touchDetector.IsSliding) || _canJumpAgain)
                {
                    Rb.Rigidbody.drag = 0f;
                    Rb.Rigidbody.AddForce(Vector2.up * jumpForce * Runner.DeltaTime, ForceMode2D.Impulse);

                    if (_canJumpAgain)
                    {
                        DoubleJump = false;
                    }
                }

                AnimationHandler.SetJumpAnimation(pressedButtons);
            }
        }

        public void BetterJumpLogic(NetworkInputData input, CharacterTouchDetector touchDetector)
        {
            if (touchDetector.IsGrounded) return;

            if (IsFallingDown())
            {
                if (touchDetector.IsSliding && input.AxisPressed())
                {
                    Rb.Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (wallSlidingMultiplier - 1) *
                                              Runner.DeltaTime;
                }
                else
                {
                    Rb.Rigidbody.velocity +=
                        Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Runner.DeltaTime;
                }
            }
            else if (Rb.Rigidbody.velocity.y > 0f && !input.GetButton(InputButton.Jump))
            {
                Rb.Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Runner.DeltaTime;
            }
        }

        public void LimitSpeed()
        {
            if (Mathf.Abs(Rb.Rigidbody.velocity.x) > maxVelocity)
            {
                Rb.Rigidbody.velocity *= horizontalVelocityReduction;
            }

            if (Mathf.Abs(Rb.Rigidbody.velocity.y) > maxVelocity)
            {
                Rb.Rigidbody.velocity *= verticalVelocityReduction;
            }
        }

        public void Slide(CharacterTouchDetector touchDetector)
        {
            if (touchDetector.IsSliding)
            {
                Rb.Rigidbody.velocity = new Vector2(Rb.Rigidbody.velocity.x,
                    Mathf.Clamp(Rb.Rigidbody.velocity.y, -wallSlidingMultiplier, float.MaxValue));
            }
        }

        public void JumpDown(NetworkButtons pressedButtons, CharacterCollisionHandler collisionHandler)
        {
            if (pressedButtons.IsSet(InputButton.JumpDown))
            {
                collisionHandler.ExcludePlatformCollider();
            }
        }
        
        public void SetDrag()
        {
            Rb.Rigidbody.drag = drag;
            AnimationHandler.SetMovementAnimation(false);
        }

        public void ResetDrag()
        {
            Rb.Rigidbody.drag = 0f;
        }
        
        public bool IsFallingDown()
        {
            return Rb.Rigidbody.velocity.y < 0f;
        }

        public void EnableDoubleJump()
        {
            DoubleJump = true;
        }

        public void EnablePhysics(bool enable)
        {
            Rb.Rigidbody.simulated = enable;
        }

        public void SetSlow(float slowTime, float slowMultiplier)
        {
            SlowTimer = TickTimer.CreateFromSeconds(Runner, slowTime);
            _currentAcceleration = acceleration * slowMultiplier;
        }
    }
}