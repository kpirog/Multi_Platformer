using System;
using Fusion;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    
    public static event Action<GameState> OnGameStateChanged;

    [Networked(OnChanged = nameof(OnStateChanged))]
    public GameState State { get; private set; }
    
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