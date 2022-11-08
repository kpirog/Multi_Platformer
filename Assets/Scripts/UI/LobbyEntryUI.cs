using TMPro;
using UnityEngine;
using NetworkPlayer = GDT.Network.NetworkPlayer;

namespace GDT.UI
{
    public class LobbyEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private TextMeshProUGUI readyText;

        private NetworkPlayer _displayedPlayer;
        
        public void SetPlayer(NetworkPlayer player)
        {
            _displayedPlayer = player;
        }

        private void Update()
        {
            nicknameText.text = _displayedPlayer.NickName.Value;
            readyText.text = _displayedPlayer.Ready ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
        }
    }
}