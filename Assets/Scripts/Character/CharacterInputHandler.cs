using System;
using System.Collections;
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

        private bool _inputAllowed;
        private bool _reversedControl;
        private float _shootingAngle;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            //_inputAllowed = false;
            _inputAllowed = true;
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Runner.AddCallbacks(this);

                GameManager.OnGameStateChanged += SetInputAllowed;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) && _inputAllowed)
            {
                Vector2 shootDirection = (GetMousePosition() - transform.position).normalized;
                _shootingAngle = Vector3.SignedAngle(shootDirection, transform.right, Vector3.back);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (!_inputAllowed) return;

            NetworkInputData inputData = new NetworkInputData();

            inputData.Buttons.Set(_reversedControl ? InputButton.Right : InputButton.Left, Input.GetKey(KeyCode.A));
            inputData.Buttons.Set(_reversedControl ? InputButton.Left : InputButton.Right, Input.GetKey(KeyCode.D));
            inputData.Buttons.Set(InputButton.Jump, Input.GetKey(KeyCode.Space));
            inputData.Buttons.Set(InputButton.Shoot, Input.GetMouseButton(0));
            inputData.Buttons.Set(InputButton.StandardArrow, Input.GetKey(KeyCode.Alpha1));
            inputData.Buttons.Set(InputButton.IceArrow, Input.GetKey(KeyCode.Alpha2));
            inputData.ShootingAngle = _shootingAngle;

            input.Set(inputData);
        }

        private void SetInputAllowed(GameState state)
        {
            if (state != GameState.Playing && _inputAllowed) return;
            _inputAllowed = true;
        }

        public IEnumerator TurnOffInputForSeconds(float time)
        {
            _inputAllowed = false;
            yield return new WaitForSeconds(time);
            _inputAllowed = true;
        }

        public IEnumerator ReverseControlForSeconds(float time)
        {
            _reversedControl = true;
            yield return new WaitForSeconds(time);
            _reversedControl = false;
        }

        private Vector3 GetMousePosition()
        {
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