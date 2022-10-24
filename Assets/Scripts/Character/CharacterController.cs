using Fusion;
using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterController : NetworkBehaviour
    {
        private CharacterMovementHandler _movementHandler;
        private CharacterInputHandler _inputHandler;
        private CharacterTouchDetector _touchDetector;
        private CharacterShootingController _shootingController;
        private CharacterAnimationHandler _animationHandler;
        
        
        private void Awake()
        {
            _movementHandler = GetComponent<CharacterMovementHandler>();
            _inputHandler = GetComponent<CharacterInputHandler>();
            _touchDetector = GetComponent<CharacterTouchDetector>();
            _shootingController = GetComponent<CharacterShootingController>();
            _animationHandler = GetComponent<CharacterAnimationHandler>();
        }

        public override void FixedUpdateNetwork()
        {
            _movementHandler.LimitSpeed();
            HandleFallDown();
            HandleSlide();

            if (GetInput(out NetworkInputData input))
            {
                var pressedButtons = input.Buttons.GetPressed(_inputHandler.PreviousButtons);
                var releasedButtons = input.Buttons.GetReleased(_inputHandler.PreviousButtons);
                _inputHandler.PreviousButtons = input.Buttons;
                
                HandleMovement(input);
                HandleJump(pressedButtons, input);
                HandleShoot(input, releasedButtons, input.ShootingAngle);
            }
        }

        private void HandleFallDown()
        {
            _animationHandler.SetFallDownAnimation(!_touchDetector.IsGrounded && _movementHandler.IsFallingDown());
        }
        
        private void HandleJump(NetworkButtons pressedButtons, NetworkInputData input)
        {
            _movementHandler.Jump(pressedButtons, _touchDetector);
            _movementHandler.BetterJumpLogic(input, _touchDetector);
        }

        private void HandleMovement(NetworkInputData input)
        {
            Vector2 direction = GetMovementDirection(input);

            if (direction != Vector2.zero)
            {
                _movementHandler.Move(input);
            }
            else
            {
                _movementHandler.SetDrag();
            }
        }

        private void HandleSlide()
        {
            _movementHandler.Slide(_touchDetector);
        }

        private void HandleShoot(NetworkInputData input, NetworkButtons releasedButtons, float angle)
        {
            if (input.GetButton(InputButton.Shoot))
            {
                _shootingController.StretchBow();
            }

            if (releasedButtons.IsSet(InputButton.Shoot))
            {
                _shootingController.ReleaseArrow(angle);
            }
        }

        private Vector2 GetMovementDirection(NetworkInputData input)
        {
            if (input.GetButton(InputButton.Left))
            {
                return Vector2.left;
            }
            
            if (input.GetButton(InputButton.Right))
            {
                return Vector2.right;
            }
            
            return Vector2.zero;
        }
    }
}