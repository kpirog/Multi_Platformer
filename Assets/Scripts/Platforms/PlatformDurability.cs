using Fusion;
using UnityEngine;
using Medicine;

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
        
        [Inject] private NetworkRigidbody2D Rb { get; }
        [Inject] private Collider2D Collider { get; }
        [Inject] private PlatformEffector2D PlatformEffector { get; }
        private TickTimer DurabilityTimer { get; set; }
        
        private LayerMask _untouchableLayer;
        
        private void Start()
        {
            Rb.Rigidbody.isKinematic = true;
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
            Rb.Rigidbody.isKinematic = false;
            Collider.usedByEffector = false;
            PlatformEffector.enabled = false;

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