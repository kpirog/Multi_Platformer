using System.Linq;
using Fusion;
using GDT.Network;
using UnityEngine;

public class ResultChecker : NetworkBehaviour
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
        if (!Object.HasStateAuthority) return;

        //var winner = NetworkSpawner.SpawnedCharacters.Select(x => x.Value)
            //.OrderByDescending(x => x.GetCurrentHeight()).First();

        //ameManager.Instance.SetWinner(winner);
    }
}