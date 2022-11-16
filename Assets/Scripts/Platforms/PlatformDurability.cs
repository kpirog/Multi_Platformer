using Fusion;
using UnityEngine;

namespace GDT.Platforms
{
    [RequireComponent(typeof(NetworkRigidbody2D))]
    public class PlatformDurability : SimulationBehaviour
    {
        [SerializeField] private float durability;
        [SerializeField] private float durabilityTime;
        [SerializeField] private float damageSpeed;

        [SerializeField] private SpriteRenderer[] spriteRenderers;
        [SerializeField] private Color destroyedColor;
        
        private LayerMask _untouchableLayer;
        private NetworkRigidbody2D _rb;
        private Collider2D _collider;
        private PlatformEffector2D _platformEffector;

        private TickTimer DurabilityTimer { get; set; }

        private void Awake()
        {
            _rb = GetComponent<NetworkRigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _platformEffector = GetComponent<PlatformEffector2D>();
        }

        private void Start()
        {
            _rb.Rigidbody.isKinematic = true;
            _untouchableLayer = LayerMask.NameToLayer("Untouchable");
        }

        public override void FixedUpdateNetwork()
        {
            if (DurabilityTimer.Expired(Runner))
            {
                SetPlatformDestroyed();
            }
        }

        public void DecreaseDurability(float deltaTime) //Different solution
        {
            if (durability > 0)
            {
                durability -= deltaTime * damageSpeed;
                Debug.Log($"Current durability = {durability}");
                return;
            }

            SetPlatformDestroyed();
        }

        private void SetPlatformDestroyed()
        {
            Debug.Log($"Set platform destroyed!");
            gameObject.layer = _untouchableLayer;
            _rb.Rigidbody.isKinematic = false;
            _collider.usedByEffector = false;
            _platformEffector.enabled = false;

            foreach (var sprite in spriteRenderers)
            {
                sprite.color = destroyedColor;
            }
        }

        public void StartDestroying()
        {
            if (DurabilityTimer.IsRunning || durabilityTime <= 0f) return;
            Debug.Log($"Start destroying: {durabilityTime}");
            DurabilityTimer = TickTimer.CreateFromSeconds(Runner, durabilityTime);
        }

        public void StopDestroying()
        {
            if (!DurabilityTimer.IsRunning) return;
            durabilityTime = DurabilityTimer.RemainingTime(Runner).Value;
            DurabilityTimer = TickTimer.None;
        }
    }
}