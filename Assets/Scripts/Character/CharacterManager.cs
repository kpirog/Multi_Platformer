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
        private CharacterInputHandler _inputHandler;
        
        private void Awake()
        {
            _movementHandler = GetComponent<CharacterMovementHandler>();
            _animationHandler = GetComponent<CharacterAnimationHandler>();
            _inputHandler = GetComponent<CharacterInputHandler>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData input))
            {
                if (input.MovementDirection.sqrMagnitude > 0f)
                {
                    _movementHandler.Move(input.MovementDirection);
                    
                    _animationHandler.SetMovementAnimation(true);
                    _animationHandler.SetSpriteDirection(input.MovementDirection);
                }
                else
                {
                    _animationHandler.SetMovementAnimation(false);
                    _movementHandler.Stop();
                }
            }
        }
    }
}