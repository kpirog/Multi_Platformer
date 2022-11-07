using System;
using Fusion;
using GDT.Network;
using UnityEngine;
using NetworkPlayer = GDT.Network.NetworkPlayer;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public static event Action<GameState> OnGameStateChanged;

    [Networked(OnChanged = nameof(OnStateChanged))]
    public GameState State { get; private set; }

    public NetworkPlayer Winner { get; private set; }
    
    [SerializeField] private int requiredPlayersCount;
    
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

    private void Update()
    {
        EnableInputs();
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

    private void EnableInputs()
    {
        if (NetworkSpawner.SpawnedCharacters.Count >= requiredPlayersCount && State != GameState.Playing)
        {
            Debug.Log("Wykonuje sie");
            SetGameState(GameState.Playing);            
        }
    }
}

public enum GameState
{
    Lobby,
    Playing,
    Finished,
    Loading
}