using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using GDT.Common;
using GDT.Data;
using UnityEngine;

namespace GDT.Character
{
    public class CharacterInputHandler : NetworkBehaviour, INetworkRunnerCallbacks
    {
        [Networked] private TickTimer FreezeInputTimer { get; set; }
        [Networked] private TickTimer InvertInputTimer { get; set; }
        [Networked] private NetworkBool InputFrozen { get; set; }
        [Networked] private NetworkBool InputInverted { get; set; }
        [Networked] public NetworkButtons PreviousButtons { get; set; }
        
        private float _shootingAngle;
        private Camera _mainCamera;
        private Vector2 _shootDirection;
        private Vector2 _mousePosition;

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Runner.AddCallbacks(this);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (InputFrozen && FreezeInputTimer.Expired(Runner))
            {
                InputFrozen = false;
            }
            
            if (InputInverted && InvertInputTimer.Expired(Runner))
            {
                InputInverted = false;
            }
        }

        private void Update()
        {
            if (InputFrozen) return;

            if (Input.GetMouseButton(0))
            {
                _shootDirection = (GetMousePosition() - transform.position).normalized;
                _shootingAngle = Vector3.SignedAngle(_shootDirection, transform.right, Vector3.back);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _mousePosition = GetMousePosition();
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var inputData = new NetworkInputData();

            if (!GameManager.Instance || InputFrozen) return;

            switch (GameManager.Instance.State)
            {
                default:
                case GameState.Lobby:
                    inputData.Buttons.Set(InputButton.Ready, Input.GetKey(KeyCode.R));
                    break;
                case GameState.Playing:
                    inputData.Buttons.Set(InputInverted ? InputButton.Right : InputButton.Left,
                        Input.GetKey(KeyCode.A));
                    inputData.Buttons.Set(InputInverted ? InputButton.Left : InputButton.Right,
                        Input.GetKey(KeyCode.D));
                    inputData.Buttons.Set(InputButton.Jump, Input.GetKey(KeyCode.Space));
                    inputData.Buttons.Set(InputButton.Shoot, Input.GetMouseButton(0));
                    inputData.Buttons.Set(InputButton.StandardArrow, Input.GetKey(KeyCode.Alpha1));
                    inputData.Buttons.Set(InputButton.IceArrow, Input.GetKey(KeyCode.Alpha2));
                    inputData.Buttons.Set(InputButton.InvertedArrow, Input.GetKey(KeyCode.Alpha3));
                    inputData.Buttons.Set(InputButton.JumpDown, Input.GetKey(KeyCode.S));
                    inputData.Buttons.Set(InputButton.GrapplingHook, Input.GetKey(KeyCode.E));
                    inputData.Buttons.Set(InputButton.DecreaseRope, Input.GetKey(KeyCode.W));
                    inputData.Buttons.Set(InputButton.W, Input.GetKey(KeyCode.W));
                    inputData.Buttons.Set(InputButton.IncreaseRope, Input.GetKey(KeyCode.S));
                    inputData.ShootingAngle = _shootingAngle;
                    inputData.MousePosition = _mousePosition;
                    break;
            }

            input.Set(inputData);
        }

        public void InvertControlForSeconds(float time)
        {
            InvertInputTimer = TickTimer.CreateFromSeconds(Runner, time);
            InputInverted = true;
        }

        public void FreezeInputForSeconds(float freezeTime)
        {
            FreezeInputTimer = TickTimer.CreateFromSeconds(Runner, freezeTime);
            InputFrozen = true;
        }

        public Vector3 GetMousePosition()
        {
            if (!_mainCamera)
            {
                _mainCamera = Camera.main;
            }

            Vector3 screenPosition = Input.mousePosition;
            Vector3 worldPosition =
                _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y,
                    _mainCamera.nearClipPlane));

            return worldPosition;
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