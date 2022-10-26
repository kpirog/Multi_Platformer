using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GDT.Network
{
    public class NetworkRunnerHandler : MonoBehaviour
    {
        private NetworkRunner _networkRunner;
        private NetworkSceneManagerDefault _sceneManager;

        private bool _initialized;

        [SerializeField] private GameManager gameManager;

        private void Awake()
        {
            _networkRunner = GetComponent<NetworkRunner>();
            _sceneManager = GetComponent<NetworkSceneManagerDefault>();

            DontDestroyOnLoad(gameObject);
        }

        public void StartGame()
        {
            InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, "TestSession",
                SceneManager.GetActiveScene().buildIndex + 1);
        }

        private async void InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName,
            SceneRef sceneRef)
        {
            runner.ProvideInput = true;

            await runner.StartGame(new StartGameArgs()
                {
                    GameMode = gameMode,
                    SessionName = sessionName,
                    Scene = sceneRef,
                    SceneManager = _sceneManager,
                }
            );

            runner.Spawn(gameManager);
        }
    }
}