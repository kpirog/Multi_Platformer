using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using NetworkPlayer = GDT.Network.NetworkPlayer;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private NetworkPlayer networkPlayerPrefab;
    [SerializeField] private Vector2[] temporarySpawnPositions;

    private static readonly HashSet<NetworkPlayer> Players = new();
    private static readonly Dictionary<PlayerRef, NetworkPlayer> PlayersByRef = new();

    public static event Action<NetworkPlayer> OnPlayerRegistered;
    public static event Action<NetworkPlayer> OnPlayerUnregistered;
    
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

    public static void SpawnPlayer(NetworkRunner runner, PlayerRef playerRef)
    {
        var player = runner.Spawn(Instance.networkPlayerPrefab, Instance.temporarySpawnPositions[Players.Count], Quaternion.identity, playerRef);
    }

    public static void DespawnPlayer(NetworkRunner runner, PlayerRef playerRef)
    {
        runner.Despawn(PlayersByRef[playerRef].Object);
    }

    public static void RegisterPlayer(NetworkPlayer player)
    {
        Players.Add(player);

        if (player.Object.InputAuthority != PlayerRef.None)
        {
            PlayersByRef.Add(player.Object.InputAuthority, player);
        }
        
        OnPlayerRegistered?.Invoke(player);
    }

    public static void UnregisterPlayer(NetworkPlayer player)
    {
        Players.Remove(player);
        
        if (player.Object.InputAuthority != PlayerRef.None)
        {
            PlayersByRef.Remove(player.Object.InputAuthority);
        }
        
        OnPlayerUnregistered?.Invoke(player);
    }
}
