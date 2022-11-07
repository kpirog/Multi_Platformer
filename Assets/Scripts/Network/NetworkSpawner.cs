using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using GDT.Character;
using UnityEngine;

namespace GDT.Network
{
    public class NetworkSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkPlayer networkPlayerPrefab;
        [SerializeField] private Vector2[] temporarySpawnPositions;

        private static Dictionary<PlayerRef, NetworkPlayer> _spawnedCharacters;
        private CharacterInputHandler _characterInputHandler;
        
        private int _spawnIndex;
        public static Dictionary<PlayerRef, NetworkPlayer> SpawnedCharacters => _spawnedCharacters;

        private void Awake()
        {
            _spawnedCharacters = new Dictionary<PlayerRef, NetworkPlayer>();
            _spawnIndex = 0;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                var networkObject = runner.Spawn(networkPlayerPrefab, temporarySpawnPositions[_spawnIndex], Quaternion.identity, player);
                _spawnedCharacters.Add(player, networkObject);
                networkObject.name = "Player_" + _spawnIndex;
                _spawnIndex++;
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_spawnedCharacters.TryGetValue(player, out var networkPlayer))
            {
                runner.Despawn(networkPlayer.Object);
                _spawnedCharacters.Remove(player);
                _spawnIndex--;
            }
        }
        
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}
        public void OnConnectedToServer(NetworkRunner runner) {}
        public void OnDisconnectedFromServer(NetworkRunner runner) {}
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }

    }
}