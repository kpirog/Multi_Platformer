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
        }

        if (Input.GetKeyDown(KeyCode.P)) //Temp
        {
            GameManager.Instance.SetGameState(GameState.Finished);
        }
    }

    private void HandleCounting(GameState state)
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
    
    private void StartCounting()
    {
        _isCounting = true;
        _startTime = Time.time;
    }

    private void StopCounting()
    {
        _isCounting = false;
    }
}