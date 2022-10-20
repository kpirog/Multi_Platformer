using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterInputHandler : MonoBehaviour
    {
        private Vector2 _movementDirection;
        
        private void Update()
        {
            _movementDirection.x = Input.GetAxis("Horizontal");
        }

        public NetworkInputData GetNetworkData()
        {
            var networkInputData = new NetworkInputData
            {
                MovementDirection = _movementDirection
            };

            return networkInputData;
        }
    }
}