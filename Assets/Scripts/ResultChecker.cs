using System.Linq;
using GDT.Network;
using UnityEngine;

public class ResultChecker : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += CheckWinner;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= CheckWinner;
    }

    private void CheckWinner(GameState state)
    {
        if (state != GameState.Finished) return;

        var winner = NetworkSpawner.SpawnedCharacters.Select(x => x.Value)
            .OrderByDescending(x => x.GetCurrentHeight()).First();
        
        GameManager.Instance.SetWinner(winner);
    }
}
