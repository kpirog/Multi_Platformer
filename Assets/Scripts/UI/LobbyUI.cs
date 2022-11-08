using System.Collections.Generic;
using UnityEngine;
using NetworkPlayer = GDT.Network.NetworkPlayer;

namespace GDT.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private Transform clientEntryParent;
        [SerializeField] private ClientEntryUI clientEntryPrefab;

        private readonly Dictionary<NetworkPlayer, ClientEntryUI> _clientEntries = new();

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

            var clientEntry = Instantiate(clientEntryPrefab, clientEntryParent);
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