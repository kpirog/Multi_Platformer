using System;
using Fusion;
using GDT.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GDT.Network
{
    public class NetworkRunnerHandler : MonoBehaviour
    {
        public static NetworkRunnerHandler Instance;
        
        [SerializeField] private GameManager gameManager;

        private NetworkRunner _networkRunner;
        private NetworkSceneManagerDefault _sceneManager;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            _networkRunner = GetComponent<NetworkRunner>();
            _sceneManager = GetComponent<NetworkSceneManagerDefault>();
        }

        private void Update()
        {
            if (_networkRunner != null && Input.GetKeyDown(KeyCode.G))
            {
                _networkRunner.SetActiveScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        public void JoinGameAsHost(string sessionName)
        {
            InitializeNetworkRunner(_networkRunner, GameMode.Host, sessionName, _sceneManager);
        }
        
        public void JoinGameAsClient(string sessionName)
        {
            InitializeNetworkRunner(_networkRunner, GameMode.Client, sessionName, _sceneManager);
        }

        private async void InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName, INetworkSceneManager sceneManager)
        {
            runner.ProvideInput = true;

            await runner.StartGame(new StartGameArgs()
                {
                    GameMode = gameMode,
                    SessionName = sessionName,
                    SceneManager = sceneManager
                }
            );

            runner.Spawn(gameManager);
        }
    }
}