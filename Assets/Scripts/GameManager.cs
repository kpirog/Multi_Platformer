using System;
using Fusion;
using UnityEngine;
using NetworkPlayer = GDT.Network.NetworkPlayer;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private int requiredPlayersCount;
    
    public static event Action<GameState> OnGameStateChanged;

    [Networked(OnChanged = nameof(OnStateChanged))]
    public GameState State { get; private set; } = GameState.Lobby;

    public NetworkPlayer Winner { get; private set; }
    
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
    
    public void SetGameState(GameState state)
    {
        State = state;
    }

    public void SetWinner(NetworkPlayer winner)
    {
        Debug.Log($"The winner is {winner.gameObject.name}");
        
        Winner = winner;
    }

    public static void OnStateChanged(Changed<GameManager> changed)
    {
        OnGameStateChanged?.Invoke(changed.Behaviour.State);
    }
}

public enum GameState
{
    Lobby,
    Playing,
    Finished,
    Loading
}