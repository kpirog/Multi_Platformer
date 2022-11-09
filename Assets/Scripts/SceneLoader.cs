using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    
    [SerializeField] private NetworkRunner networkRunner;

    private SceneRef _gameScene;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _gameScene = SceneManager.GetActiveScene().buildIndex + 1;
    }

    public static void LoadGameScene()
    {
        Instance.networkRunner.SetActiveScene(Instance._gameScene);
    }
}
