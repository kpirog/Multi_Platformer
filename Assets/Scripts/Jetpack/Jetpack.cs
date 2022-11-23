using System;
using Fusion;
using GDT.Data;
using Medicine;
using UnityEngine;

namespace GDT.Jetpack
{
    public class Jetpack : NetworkBehaviour
    {
        [SerializeField] private float force;
        [SerializeField] private float fuel;
        [SerializeField] private LayerMask layerMask;
        [Inject] private Collider2D Collider { get; }
        [Inject.FromChildren] private ParticleSystem Particles { get; }

        private bool _equipped;


        public override void FixedUpdateNetwork()
        {
            var collider = Runner.GetPhysicsScene2D()
                .OverlapBox(Collider.bounds.center, Collider.bounds.size, 0f, layerMask);

            if (!collider) return;
            var player = collider.gameObject.GetComponentInParent<NetworkRigidbody2D>();
            
            if (!_equipped)
            {
                Equip(player);
                return;
            }

            if (GetInput(out NetworkInputData input))
            {
                if (input.GetButton(InputButton.W))
                {
                    player.Rigidbody.AddForce(Vector2.up * force, ForceMode2D.Force);
                    fuel -= Runner.DeltaTime;
                    Particles.Play();
                }
                else
                {
                    if (Particles.isPlaying)
                    {
                        Particles.Stop();
                    }
                }
            }
            
            if (fuel <= 0f)
            {
                Runner.Despawn(Object);
            }
        }

        private void Equip(NetworkRigidbody2D parent)
        {
            _equipped = true;
            transform.SetParent(parent.transform);
            transform.localPosition = Vector3.zero;
            Object.AssignInputAuthority(parent.Object.InputAuthority);
        }
    }
}