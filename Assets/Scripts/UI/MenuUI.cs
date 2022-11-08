using System;
using GDT.Data;
using GDT.Network;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GDT.UI
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nickNameInputField;
        [SerializeField] private TMP_InputField sessionNameInputField;

        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;

        [SerializeField] private GameObject menuPanel;

        private void Awake()
        {
            hostButton.onClick.AddListener(HostButton_OnClick);
            joinButton.onClick.AddListener(JoinButton_OnClick);
        }
        
        private void HostButton_OnClick()
        {
            if (AreInputFieldsEmpty()) return;
            NetworkRunnerHandler.Instance.JoinGameAsHost(sessionNameInputField.text);
            LocalPlayerDataHandler.Nickname = nickNameInputField.text;
            menuPanel.SetActive(false);
        }

        private void JoinButton_OnClick()
        {
            if (AreInputFieldsEmpty()) return;
            NetworkRunnerHandler.Instance.JoinGameAsClient(sessionNameInputField.text);
            LocalPlayerDataHandler.Nickname = nickNameInputField.text;
            menuPanel.SetActive(false);
        }
        
        private bool AreInputFieldsEmpty()
        {
            return string.IsNullOrEmpty(sessionNameInputField.text) || string.IsNullOrEmpty(nickNameInputField.text);
        }
    }
}