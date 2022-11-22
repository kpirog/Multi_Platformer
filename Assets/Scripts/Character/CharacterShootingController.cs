using Fusion;
using GDT.Data;
using GDT.Projectiles;
using UnityEngine;
using Medicine;

namespace GDT.Character
{
    public class CharacterShootingController : NetworkBehaviour
    {
        [SerializeField] private Arrow[] arrowPrefabs;
        [Networked] [UnitySerializeField] private int StandardArrowsCount { get; set; }
        [Networked] [UnitySerializeField] private int IceArrowsCount { get; set; }
        [Networked] [UnitySerializeField] private int InvertedArrowsCount { get; set; }
        
        [Networked] private float ReleaseTimer { get; set; }
        [Networked] private int CurrentArrowIndex { get; set; }
        [Networked] private int ArrowsCount { get; set; }

        [Inject] private CharacterAnimationHandler AnimationHandler { get; }
        [Inject] private TrajectoryPrediction TrajectoryPrediction { get; }

        private Arrow CurrentArrowPrefab => arrowPrefabs[CurrentArrowIndex];

        private void Start()
        {
            CurrentArrowIndex = 0;
            ArrowsCount = StandardArrowsCount;
        }

        private bool PlayerHasArrow()
        {
            return ArrowsCount > 0;
        }

        public void SetCurrentArrow(NetworkButtons pressed)
        {
            if (pressed.IsSet(InputButton.StandardArrow))
            {
                CurrentArrowIndex = 0;
                ArrowsCount = StandardArrowsCount;
            }
            else if (pressed.IsSet(InputButton.IceArrow))
            {
                CurrentArrowIndex = 1;
                ArrowsCount = IceArrowsCount;
            }
            else if (pressed.IsSet(InputButton.InvertedArrow))
            {
                CurrentArrowIndex = 2;
                ArrowsCount = InvertedArrowsCount;
            }
        }
        
        public void StretchBow(float angle)
        {
            if (!PlayerHasArrow()) return;

            ReleaseTimer += Runner.DeltaTime;

            if (ReleaseTimer >= 1f)
            {
                ReleaseTimer = 1f;
            }

            AnimationHandler.SetShootAnimation();

            if (Object.HasInputAuthority)
            {
                TrajectoryPrediction.DisplayTrajectory(ReleaseTimer, CurrentArrowPrefab.Speed,
                    CalculateShootDirection(angle));
            }
        }

        public void ReleaseArrow(float angle)
        {
            if (!PlayerHasArrow()) return;

            if (Object.HasStateAuthority)
            {
                var arrow = Runner.Spawn(arrowPrefabs[CurrentArrowIndex], transform.position, Quaternion.identity,
                    Object.InputAuthority);
                arrow.Release(CalculateShootDirection(angle), ReleaseTimer);
                RemoveArrow();
            }

            ReleaseTimer = 0f;
            AnimationHandler.StopShootAnimation();
            TrajectoryPrediction.SetVisible(false);
        }

        private void RemoveArrow()
        {
            ArrowsCount--;

            switch (CurrentArrowIndex)
            {
                case 0:
                    StandardArrowsCount = ArrowsCount;
                    break;
                case 1:
                    IceArrowsCount = ArrowsCount;
                    break;
                case 2:
                    InvertedArrowsCount = ArrowsCount;
                    break;
            }
        }

        private Vector2 CalculateShootDirection(float angle)
        {
            return Quaternion.Euler(0f, 0f, angle) * transform.right;
        }
    }
}