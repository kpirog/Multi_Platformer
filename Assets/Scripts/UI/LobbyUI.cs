using System.Collections.Generic;
using UnityEngine;
using NetworkPlayer = GDT.Network.NetworkPlayer;

namespace GDT.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private Transform lobbyEntryParent;
        [SerializeField] private LobbyEntryUI lobbyEntryPrefab;

        private readonly Dictionary<NetworkPlayer, LobbyEntryUI> _clientEntries = new();

        private void OnEnable()
        {
            PlayerManager.OnPlayerRegistered += SpawnClientEntry;
            PlayerManager.OnPlayerUnregistered += DestroyClientEntry;
        }

        private void OnDisable()
        {
            PlayerManager.OnPlayerRegistered -= SpawnClientEntry;
            PlayerManager.OnPlayerUnregistered -= DestroyClientEntry;
        }
        
        private void SpawnClientEntry(NetworkPlayer player)
        {
            if (!lobbyPanel.activeSelf)
            {
                lobbyPanel.SetActive(true);
            }

            var clientEntry = Instantiate(lobbyEntryPrefab, lobbyEntryParent);
            clientEntry.SetPlayer(player);
            
            _clientEntries.Add(player, clientEntry);
        }

        private void DestroyClientEntry(NetworkPlayer player)
        {
            Destroy(_clientEntries[player].gameObject);
            _clientEntries.Remove(player);
        }
    }
}