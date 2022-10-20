using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GDT.Network
{
    public class NetworkRunnerHandler : MonoBehaviour
    {
        private NetworkRunner _networkRunner;
        private NetworkSceneManagerDefault _sceneManager;

        private void Awake()
        {
            _networkRunner = GetComponent<NetworkRunner>();
            _sceneManager = GetComponent<NetworkSceneManagerDefault>();
        }

        private void Start()
        {
            InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, "TestSession");

            Debug.Log("Server started!");
        }

        private async void InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName)
        {
            runner.ProvideInput = true;

            await runner.StartGame(new StartGameArgs()
            {
                GameMode = gameMode,
                SessionName = sessionName,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = _sceneManager
            });
        }
    }
}