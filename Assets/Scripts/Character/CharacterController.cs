using Fusion;
using GDT.Common;
using GDT.Data;
using GDT.Grappling;
using UnityEngine;
using NetworkPlayer = GDT.Network.NetworkPlayer;

namespace GDT.Character
{
    public class CharacterController : NetworkBehaviour
    {
        [SerializeField] private GameObject model;

        private CharacterTouchDetector _touchDetector;
        private CharacterShootingController _shootingController;
        private CharacterAnimationHandler _animationHandler;
        private NetworkPlayer _player;

        [HideInInspector] public CharacterMovementHandler movementHandler;
        [HideInInspector] public CharacterInputHandler inputHandler;
        [HideInInspector] public CharacterCollisionHandler collisionHandler;
        
        private void Awake()
        {
            movementHandler = GetComponent<CharacterMovementHandler>();
            inputHandler = GetComponent<CharacterInputHandler>();
            _touchDetector = GetComponent<CharacterTouchDetector>();
            _shootingController = GetComponent<CharacterShootingController>();
            _animationHandler = GetComponent<CharacterAnimationHandler>();
            collisionHandler = GetComponent<CharacterCollisionHandler>();
            _player = GetComponent<NetworkPlayer>();
        }

        public override void Spawned()
        {
            movementHandler.EnablePhysics(false);
            SetModelVisible(false);

            GameManager.OnGameStateChanged += SetInteractable;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            GameManager.OnGameStateChanged -= SetInteractable;
        }

        public override void FixedUpdateNetwork()
        {
            movementHandler.LimitSpeed();
            HandleFallDown();
            HandleSlide();

            if (!GetInput(out NetworkInputData input)) return;

            var pressedButtons = input.Buttons.GetPressed(inputHandler.PreviousButtons);
            var releasedButtons = input.Buttons.GetReleased(inputHandler.PreviousButtons);
            inputHandler.PreviousButtons = input.Buttons;

            HandleMovement(input);
            HandleDrag(releasedButtons);
            HandleJump(pressedButtons, input);
            HandleJumpDown(pressedButtons);
            HandleShoot(input, releasedButtons, input.ShootingAngle);
            SwitchArrow(pressedButtons);

            if (pressedButtons.IsSet(InputButton.Ready))
            {
                _player.ToggleReady();
            }
        }
        private void SwitchArrow(NetworkButtons pressedButtons)
        {
            _shootingController.SetCurrentArrow(pressedButtons);
        }

        private void HandleFallDown()
        {
            _animationHandler.SetFallDownAnimation(!_touchDetector.IsGrounded && movementHandler.IsFallingDown());
        }

        private void HandleJump(NetworkButtons pressedButtons, NetworkInputData input)
        {
            movementHandler.Jump(pressedButtons, _touchDetector);
            movementHandler.BetterJumpLogic(input, _touchDetector);
        }

        private void HandleJumpDown(NetworkButtons pressedButtons)
        {
            movementHandler.JumpDown(pressedButtons, collisionHandler);
        }

        private void HandleMovement(NetworkInputData input)
        {
            var direction = GetMovementDirection(input);

            if (direction != Vector2.zero)
            {
                movementHandler.Move(input);
            }
        }

        private void HandleDrag(NetworkButtons releasedButtons)
        {
            if ((releasedButtons.IsSet(InputButton.Left) || releasedButtons.IsSet(InputButton.Right)) &&
                _touchDetector.IsGrounded)
            {
                movementHandler.SetDrag();
            }
            else
            {
                movementHandler.ResetDrag();
            }
        }

        private void HandleSlide()
        {
            movementHandler.Slide(_touchDetector);
        }

        private void HandleShoot(NetworkInputData input, NetworkButtons releasedButtons, float angle)
        {
            if (input.GetButton(InputButton.Shoot))
            {
                _shootingController.StretchBow(angle);
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

        private void SetModelVisible(bool visible)
        {
            model.gameObject.SetActive(visible);
        }

        private void SetInteractable(GameState state)
        {
            if (state != GameState.Preparing) return;

            movementHandler.EnablePhysics(true);
            SetModelVisible(true);
        }
    }
}