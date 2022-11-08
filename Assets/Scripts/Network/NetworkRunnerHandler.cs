using Fusion;
using UnityEngine;

namespace GDT.Network
{
    public class NetworkRunnerHandler : MonoBehaviour
    {
        public static NetworkRunnerHandler Instance;
        
        [SerializeField] private GameManager gameManager;

        private NetworkRunner _networkRunner;
        
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
        }
        
        public void JoinGameAsHost(string sessionName)
        {
            InitializeNetworkRunner(_networkRunner, GameMode.Host, sessionName);
        }
        
        public void JoinGameAsClient(string sessionName)
        {
            InitializeNetworkRunner(_networkRunner, GameMode.Client, sessionName);
        }

        private async void InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName)
        {
            runner.ProvideInput = true;

            await runner.StartGame(new StartGameArgs()
                {
                    GameMode = gameMode,
                    SessionName = sessionName
                }
            );

            runner.Spawn(gameManager);
        }
    }
}