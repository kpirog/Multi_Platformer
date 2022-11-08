using TMPro;
using UnityEngine;
using NetworkPlayer = GDT.Network.NetworkPlayer;

namespace GDT.UI
{
    public class ClientEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nicknameText;

        private NetworkPlayer _displayedPlayer;
        
        public void SetPlayer(NetworkPlayer player)
        {
            _displayedPlayer = player;
        }

        private void Update()
        {
            nicknameText.text = _displayedPlayer.NickName.Value;
        }
    }
}