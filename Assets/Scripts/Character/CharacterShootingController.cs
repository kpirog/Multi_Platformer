using Fusion;
using GDT.Data;
using GDT.Projectiles;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterShootingController : NetworkBehaviour
    {
        [SerializeField] private Arrow[] arrowPrefabs;

        private float _releaseTimer;
        private int _currentArrowIndex;

        [Networked][UnitySerializeField]
        private int StandardArrowsCount { get; set; } 
        
        [Networked][UnitySerializeField]
        private int IceArrowsCount { get; set; }

        [Networked]
        private int ArrowsCount { get; set; }

        private CharacterAnimationHandler _animationHandler;
        
        private void Awake()
        {
            _animationHandler = GetComponent<CharacterAnimationHandler>();
            _currentArrowIndex = 0;
        }
        
        private bool PlayerHasArrow()
        {
            return ArrowsCount > 0;
        }

        public void SetCurrentArrow(NetworkButtons pressed)
        {
            if (pressed.IsSet(InputButton.StandardArrow))
            {
                _currentArrowIndex = 0;
                ArrowsCount = StandardArrowsCount;
            }

            if (pressed.IsSet(InputButton.IceArrow))
            {
                _currentArrowIndex = 1;
                ArrowsCount = IceArrowsCount;
            }
        }

        public void StretchBow()
        {
            if (!PlayerHasArrow()) return;

            _releaseTimer += Runner.DeltaTime;

            if (_releaseTimer >= 1f)
            {
                _releaseTimer = 1f;
            }

            _animationHandler.SetShootAnimation();
        }

        public void ReleaseArrow(float angle)
        {
            if (!PlayerHasArrow()) return;

            if (Object.HasStateAuthority)
            {
                var arrow = Runner.Spawn(arrowPrefabs[_currentArrowIndex], transform.position, Quaternion.identity,
                    Object.InputAuthority);
                arrow.Release(angle, _releaseTimer);
                RemoveArrow();
            }

            _releaseTimer = 0f;
            _animationHandler.StopShootAnimation();
        }

        private void RemoveArrow()
        {
            ArrowsCount--;
            
            switch (_currentArrowIndex)
            {
                case 0:
                    StandardArrowsCount = ArrowsCount;
                    break;
                case 1:
                    IceArrowsCount = ArrowsCount;
                    break;
            }
        }
    }
}