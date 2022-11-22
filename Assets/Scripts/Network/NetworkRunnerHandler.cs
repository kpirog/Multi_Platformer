using Fusion;
using GDT.Common;
using UnityEngine;
using Medicine;

namespace GDT.Network
{
    public class NetworkRunnerHandler : MonoBehaviourSingleton<NetworkRunnerHandler>
    {
        [SerializeField] private GameManager gameManager;
        [Inject] private NetworkRunner NetworkRunner { get; }
        [Inject] private NetworkSceneManagerDefault SceneManager { get; }
        
        private void Update()
        {
            if (NetworkRunner != null && Input.GetKeyDown(KeyCode.G))
            {
                NetworkRunner.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        public void JoinGameAsHost(string sessionName)
        {
            InitializeNetworkRunner(NetworkRunner, GameMode.Host, sessionName, SceneManager);
        }
        
        public void JoinGameAsClient(string sessionName)
        {
            InitializeNetworkRunner(NetworkRunner, GameMode.Client, sessionName, SceneManager);
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