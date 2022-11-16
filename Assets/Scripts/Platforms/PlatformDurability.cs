using UnityEngine;

namespace GDT.Platforms
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlatformDurability : MonoBehaviour
    {
        [SerializeField] private float durability;
        [SerializeField] private float damageSpeed;

        private Rigidbody2D _rb;
        private Collider2D _collider;
        private PlatformEffector2D _platformEffector;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _platformEffector = GetComponent<PlatformEffector2D>();
            _rb.isKinematic = true;
        }

        public void DecreaseDurability(float deltaTime)
        {
            if (durability > 0)
            {
                durability -= deltaTime * damageSpeed;
                Debug.Log($"Current durability = {durability}");
                return;
            }

            _rb.isKinematic = false;
            _collider.usedByEffector = false;
            _platformEffector.enabled = false;
        }
    }
}