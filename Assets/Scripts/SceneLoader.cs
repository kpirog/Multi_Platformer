using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviourSingleton<SceneLoader>
{
    [SerializeField] private NetworkRunner networkRunner;

    private SceneRef _gameScene;

    protected override void SingletonAwakened()
    {
        _gameScene = SceneManager.GetActiveScene().buildIndex + 1;
    }

    public static void LoadGameScene()
    {
        Instance.networkRunner.SetActiveScene(Instance._gameScene);
    }
}
