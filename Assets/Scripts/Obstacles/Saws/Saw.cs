using UnityEngine;

namespace GDT.Obstacles.Saws
{
    
    public class Saw : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        
        private void Update()
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}