using Fusion;
using GDT.Common;
using GDT.Data;
using UnityEngine;
using Medicine;
using NetworkPlayer = GDT.Network.NetworkPlayer;

namespace GDT.Character
{
    public class CharacterController : NetworkBehaviour
    {
        [SerializeField] private GameObject model;

        [Inject] private CharacterTouchDetector TouchDetector { get; }
        [Inject] private CharacterShootingController ShootingController { get; }
        [Inject] private CharacterAnimationHandler AnimationHandler { get; }
        [Inject] private NetworkPlayer Player { get; }
        [Inject] public CharacterMovementHandler MovementHandler { get; }
        [Inject] public CharacterInputHandler InputHandler { get; }
        [Inject] public CharacterCollisionHandler CollisionHandler { get; }
        
        public override void Spawned()
        {
            MovementHandler.EnablePhysics(false);
            SetModelVisible(false);

            GameManager.OnGameStateChanged += SetInteractable;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            GameManager.OnGameStateChanged -= SetInteractable;
        }

        public override void FixedUpdateNetwork()
        {
            MovementHandler.LimitSpeed();
            HandleFallDown();
            HandleSlide();

            if (!GetInput(out NetworkInputData input)) return;

            var pressedButtons = input.Buttons.GetPressed(InputHandler.PreviousButtons);
            var releasedButtons = input.Buttons.GetReleased(InputHandler.PreviousButtons);
            InputHandler.PreviousButtons = input.Buttons;

            HandleMovement(input);
            HandleDrag(releasedButtons);
            HandleJump(pressedButtons, input);
            HandleJumpDown(pressedButtons);
            HandleShoot(input, releasedButtons, input.ShootingAngle);
            SwitchArrow(pressedButtons);

            if (pressedButtons.IsSet(InputButton.Ready))
            {
                Player.ToggleReady();
            }
        }
        private void SwitchArrow(NetworkButtons pressedButtons)
        {
            ShootingController.SetCurrentArrow(pressedButtons);
        }

        private void HandleFallDown()
        {
            AnimationHandler.SetFallDownAnimation(!TouchDetector.IsGrounded && MovementHandler.IsFallingDown());
        }

        private void HandleJump(NetworkButtons pressedButtons, NetworkInputData input)
        {
            MovementHandler.Jump(pressedButtons, TouchDetector);
            MovementHandler.BetterJumpLogic(input, TouchDetector);
        }

        private void HandleJumpDown(NetworkButtons pressedButtons)
        {
            MovementHandler.JumpDown(pressedButtons, CollisionHandler);
        }

        private void HandleMovement(NetworkInputData input)
        {
            var direction = GetMovementDirection(input);

            if (direction != Vector2.zero)
            {
                MovementHandler.Move(input);
            }
        }

        private void HandleDrag(NetworkButtons releasedButtons)
        {
            if ((releasedButtons.IsSet(InputButton.Left) || releasedButtons.IsSet(InputButton.Right)) &&
                TouchDetector.IsGrounded)
            {
                MovementHandler.SetDrag();
            }
            else
            {
                MovementHandler.ResetDrag();
            }
        }

        private void HandleSlide()
        {
            MovementHandler.Slide(TouchDetector);
        }

        private void HandleShoot(NetworkInputData input, NetworkButtons releasedButtons, float angle)
        {
            if (input.GetButton(InputButton.Shoot))
            {
                ShootingController.StretchBow(angle);
            }

            if (releasedButtons.IsSet(InputButton.Shoot))
            {
                ShootingController.ReleaseArrow(angle);
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

        private void SetModelVisible(bool visible)
        {
            model.gameObject.SetActive(visible);
        }

        private void SetInteractable(GameState state)
        {
            if (state != GameState.Preparing) return;

            MovementHandler.EnablePhysics(true);
            SetModelVisible(true);
        }
    }
}