using Fusion;
using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterManager : NetworkBehaviour
    {
        private CharacterMovementHandler _movementHandler;
        private CharacterInputHandler _inputHandler;
        private CharacterAnimationHandler _animationHandler;
        private CharacterTouchDetector _touchDetector;

        private void Awake()
        {
            _movementHandler = GetComponent<CharacterMovementHandler>();
            _inputHandler = GetComponent<CharacterInputHandler>();
            _animationHandler = GetComponent<CharacterAnimationHandler>();
            _touchDetector = GetComponent<CharacterTouchDetector>();
        }

        public override void FixedUpdateNetwork()
        {
            _movementHandler.LimitSpeed();
            HandleFallDown();

            if (GetInput(out NetworkInputData input))
            {
                var pressedButtons = input.Buttons.GetPressed(_inputHandler.PreviousButtons);
                _inputHandler.PreviousButtons = input.Buttons;
                
                HandleMovement(input);
                HandleJump(pressedButtons, input);
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
                
                _animationHandler.SetMovementAnimation(true);
                _animationHandler.SetSpriteDirection(direction);
            }
            else
            {
                _movementHandler.SetDrag();
                _animationHandler.SetMovementAnimation(false);
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