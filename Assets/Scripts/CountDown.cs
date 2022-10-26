using UnityEngine;

public class CountDown : MonoBehaviour
{
    private bool _isCounting;
    
    private float _currentTime;
    private float _startTime;
    
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleCounting;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleCounting;
    }

    private void Update()
    {
        if (_isCounting)
        {
            _currentTime = Time.time - _startTime;
            Debug.Log($"Current time = {_currentTime}");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.SetGameState(GameState.Finished);
        }
    }

    public void HandleCounting(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                StartCounting();
                break;
            case GameState.Finished:
                StopCounting();
                break;
        }
    }
    
    public void StartCounting()
    {
        _isCounting = true;
        _startTime = Time.time;
    }

    public void StopCounting()
    {
        _isCounting = false;
        Debug.Log($"Finish time = {_currentTime}");
    }
}