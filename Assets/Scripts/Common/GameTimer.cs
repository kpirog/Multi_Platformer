using Fusion;
using UnityEngine;

namespace GDT.Common
{
    public class GameTimer : NetworkBehaviour
    {
        [SerializeField] private float gameplayTime;
        [Networked] private TickTimer Timer { get; set; }
        
        public bool Finished => Timer.Expired(Runner);

        public float? RemainingTime => Timer.RemainingTime(Runner);
        
        public void StartCounting()
        {
            Timer = TickTimer.CreateFromSeconds(Runner, gameplayTime);
        }
    }
}