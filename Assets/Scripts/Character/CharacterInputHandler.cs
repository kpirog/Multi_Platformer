using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterInputHandler : NetworkBehaviour, INetworkRunnerCallbacks
    {
        public NetworkButtons PreviousButtons { get; set; }
        
        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Runner.AddCallbacks(this);
            }
        }
        
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            NetworkInputData inputData = new NetworkInputData();
            
            inputData.Buttons.Set(InputButton.Left, Input.GetKey(KeyCode.A));
            inputData.Buttons.Set(InputButton.Right, Input.GetKey(KeyCode.D));
            inputData.Buttons.Set(InputButton.Jump, Input.GetKey(KeyCode.Space));

            input.Set(inputData);
        }

        #region Useless code
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
        #endregion
    }
}