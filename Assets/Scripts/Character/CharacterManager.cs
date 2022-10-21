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
        private CharacterWallGroundChecker _wallGroundChecker;

        private void Awake()
        {
            _movementHandler = GetComponent<CharacterMovementHandler>();
            _animationHandler = GetComponent<CharacterAnimationHandler>();
            _wallGroundChecker = GetComponent<CharacterWallGroundChecker>();
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
            _animationHandler.SetFallDownAnimation(!_wallGroundChecker.IsGrounded && _movementHandler.IsFallingDown());
        }

        private void HandleSlide()
        {
            if (_wallGroundChecker.IsSliding)
            {
                _movementHandler.Slide();
            }
        }
        
        private void HandleJump(NetworkInputData input)
        {
            if (input.IsJumpButtonPressed && (_wallGroundChecker.IsGrounded || _wallGroundChecker.IsSliding)) 
            {
                _movementHandler.Jump();
                _animationHandler.SetJumpAnimation();
            }
        }

        private void HandleMovement(NetworkInputData input)
        {
            if (input.MovementDirection.sqrMagnitude > 0f)
            {
                _movementHandler.Move(input.MovementDirection);
                _animationHandler.SetMovementAnimation(true);
                _animationHandler.SetSpriteDirection(input.MovementDirection);
            }
            else
            {
                _movementHandler.SetDrag();
                _animationHandler.SetMovementAnimation(false);
            }
        }
    }
}