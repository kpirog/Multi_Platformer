using Fusion;
using UnityEngine;
using Medicine;

namespace GDT.Teleport
{
    public class Teleport : NetworkBehaviour
    {
        [SerializeField] private Transform exit;
        [SerializeField] private LayerMask layerMask;

        [SerializeField] private float timeToTeleport;
        [SerializeField] private Color teleportColor;

        private bool _started;
        private float _startTime;

        [Inject] private Collider2D Collider { get; }
        [Inject] private SpriteRenderer SpriteRenderer { get; }

        private TickTimer TeleportTimer { get; set; }


        public override void FixedUpdateNetwork()
        {
            var playerInTrigger = Runner.GetPhysicsScene2D()
                .OverlapBox(Collider.bounds.center, Collider.bounds.size, 0f, layerMask);

            if (playerInTrigger)
            {
                var player = playerInTrigger.gameObject.GetComponentInParent<NetworkRigidbody2D>();

                if (!_started)
                {
                    TeleportTimer = TickTimer.CreateFromSeconds(Runner, timeToTeleport);
                    _started = true;
                    _startTime = Time.time;
                }
                else
                {
                    SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, teleportColor,
                        ((Time.time - _startTime) / timeToTeleport) * Runner.DeltaTime);
                }

                if (TeleportTimer.Expired(Runner) && _started)
                {
                    player.Transform.position = exit.position;
                    TeleportTimer = TickTimer.None;
                    _started = false;
                    SpriteRenderer.color = Color.white;
                }
            }
            else
            {
                if (!_started) return;
                TeleportTimer = TickTimer.None;
                _started = false;
                SpriteRenderer.color = Color.white;
            }
        }
    }
}