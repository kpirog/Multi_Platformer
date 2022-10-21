using System;
using Fusion;
using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterManager : NetworkBehaviour
    {
        private CharacterMovementHandler _movementHandler;
        private CharacterAnimationHandler _animationHandler;
        private CharacterTouchChecker _touchChecker;

        private void Awake()
        {
            _movementHandler = GetComponent<CharacterMovementHandler>();
            _animationHandler = GetComponent<CharacterAnimationHandler>();
            _touchChecker = GetComponent<CharacterTouchChecker>();
        }

        public override void FixedUpdateNetwork()
        {
            HandleFallDown();
            HandleSlide();

            if (GetInput(out NetworkInputData input))
            {
                HandleMovement(input);
                HandleJump(input);
            }
        }

        private void HandleFallDown()
        {
            _animationHandler.SetFallDownAnimation(!_touchChecker.IsGrounded && _movementHandler.IsFallingDown());
        }

        private void HandleSlide()
        {
            if (_touchChecker.IsSliding)
            {
                _movementHandler.Slide();
            }
        }
        
        private void HandleJump(NetworkInputData input)
        {
            if (input.JumpButtonPressed && (_touchChecker.IsGrounded || _touchChecker.IsSliding)) 
            {
                _movementHandler.Jump();
                _animationHandler.SetJumpAnimation();
            }
        }

        private void HandleMovement(NetworkInputData input)
        {
            Vector2 direction = GetMovementDirection(input);

            if (direction != Vector2.zero)
            {
                _movementHandler.Move(direction);
                
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