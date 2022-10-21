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
        private CharacterGroundChecker _groundChecker;

        private void Awake()
        {
            _movementHandler = GetComponent<CharacterMovementHandler>();
            _animationHandler = GetComponent<CharacterAnimationHandler>();
            _groundChecker = GetComponent<CharacterGroundChecker>();
        }

        public override void FixedUpdateNetwork()
        {
            HandleFallDown();

            if (GetInput(out NetworkInputData input))
            {
                HandleMovement(input);
                HandleJump(input);
            }
        }

        private void HandleFallDown()
        {
            _animationHandler.SetFallDownAnimation(!_groundChecker.IsGrounded && _movementHandler.IsFallingDown());
        }

        private void HandleJump(NetworkInputData input)
        {
            if (input.IsJumpButtonPressed && _groundChecker.IsGrounded)
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