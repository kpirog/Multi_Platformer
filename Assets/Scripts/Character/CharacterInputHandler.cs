using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterInputHandler : MonoBehaviour
    {
        private Vector2 _movementDirection;

        private bool _isJumpButtonPressed;
        
        private void Update()
        {
            _movementDirection.x = Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isJumpButtonPressed = true;
            }
        }

        public NetworkInputData GetNetworkData()
        {
            var networkInputData = new NetworkInputData
            {
                MovementDirection = _movementDirection,
                IsJumpButtonPressed = _isJumpButtonPressed
            };

            _isJumpButtonPressed = false;
            
            return networkInputData;
        }
    }
}