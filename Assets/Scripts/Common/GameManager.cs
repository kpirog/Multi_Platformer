using System;
using System.Linq;
using Fusion;
using UnityEngine;
using Medicine;

namespace GDT.Common
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        public static event Action<GameState> OnGameStateChanged;

        [SerializeField] private int requiredPlayersCount;
        [SerializeField] public float timeToStartGame;
        
        [Networked(OnChanged = nameof(OnStateChanged))]
        public GameState State { get; private set; } = GameState.Lobby;
        
        [Networked] private TickTimer StartGameTimer { get; set; } 

        private bool AllPlayersReady => PlayerManager.Players.Count >= requiredPlayersCount &&
                                        PlayerManager.Players.All(x => x.Ready);
        
        [Inject] private GameTimer GameTimer { get; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;

            switch (State)
            {
                case GameState.Lobby:
                {
                    if (AllPlayersReady)
                    {
                        SceneLoader.LoadGameScene();
                        SetGameState(GameState.Preparing);
                        StartGameTimer = TickTimer.CreateFromSeconds(Runner, timeToStartGame);
                    }

                    break;
                }
                case GameState.Preparing:
                {
                    //Debug.Log($"Time to start: {StartGameTimer.RemainingTime(Runner)}");
                    
                    if (StartGameTimer.Expired(Runner))
                    {
                        SetGameState(GameState.Playing);
                        GameTimer.StartCounting();
                    }

                    break;
                }
                case GameState.Playing:
                {
                    //Debug.Log($"Time to finish: {_gameTimer.RemainingTime}");
                    
                    if (GameTimer.Finished)
                    {
                        SetGameState(GameState.Finished);
                    }
                    
                    break;
                }
                case GameState.Finished:
                {
                    //Debug.Log($"The winner is {PlayerManager.GetWinner()}");
                    break;
                }
            }
        }

        public void SetGameState(GameState state)
        {
            State = state;
        }

        public static void OnStateChanged(Changed<GameManager> changed)
        {
            OnGameStateChanged?.Invoke(changed.Behaviour.State);
        }
    }

    public enum GameState
    {
        Lobby,
        Preparing,
        Playing,
        Finished
    }
}