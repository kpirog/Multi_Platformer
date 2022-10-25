using Fusion;
using Projectiles;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterShootingController : NetworkBehaviour
    {
        [SerializeField] private Arrow arrowPrefab;

        private CharacterAnimationHandler _animationHandler;
        private float _releaseTimer;

        private void Awake()
        {
            _animationHandler = GetComponent<CharacterAnimationHandler>();
        }

        public void StretchBow()
        {
            _releaseTimer += Runner.DeltaTime;

            if (_releaseTimer >= 1f)
            {
                _releaseTimer = 1f;
            }

            _animationHandler.SetShootAnimation();
        }

        public void ReleaseArrow(float angle)
        {
            if (Object.HasStateAuthority)
            {
                var arrow = Runner.Spawn(arrowPrefab, transform.position, Quaternion.identity, Object.InputAuthority);
                arrow.Release(angle, _releaseTimer);
            }
            
            _releaseTimer = 0f;
            _animationHandler.StopShootAnimation();
        }
    }
}