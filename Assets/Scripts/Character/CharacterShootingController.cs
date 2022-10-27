using System.Collections.Generic;
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
        private List<int> _amountsOfArrows;

        private CharacterAnimationHandler _animationHandler;

        private void Awake()
        {
            _animationHandler = GetComponent<CharacterAnimationHandler>();
            _currentArrowIndex = 0;
            GetAmountOfArrows();
        }

        private void GetAmountOfArrows()
        {
            _amountsOfArrows = new List<int>();

            foreach (var arrow in arrowPrefabs)
            {
                _amountsOfArrows.Add(arrow.Amount);
            }
        }

        private bool PlayerHasArrow()
        {
            return _amountsOfArrows[_currentArrowIndex] > 0;
        }

        public void SetCurrentArrow(NetworkButtons pressed)
        {
            if (pressed.IsSet(InputButton.StandardArrow))
            {
                _currentArrowIndex = 0;
            }

            if (pressed.IsSet(InputButton.IceArrow))
            {
                _currentArrowIndex = 1;
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
                RPC_RemoveArrow();
            }

            _releaseTimer = 0f;
            _animationHandler.StopShootAnimation();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_RemoveArrow()
        {
            if (_amountsOfArrows.Count < 0) return;
            _amountsOfArrows[_currentArrowIndex]--;
        }
    }
}