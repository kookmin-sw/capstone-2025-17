using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneUI : MonoBehaviour
{
    public Button connectButton;
    public TextMeshProUGUI connectInfoTMP;

    const string CONNECT_SUCCESS = "CONNECT COMPLETED!";
    const string CONNECT_TRY = "ON CONNECTING...";

    void Awake()
    {
        // Master Server ���� �� ������ �̺�Ʈ ��� 
        NetworkManager.OnConnectedToServer += () =>
        {
            connectButton.interactable = true;
            connectInfoTMP.text = CONNECT_SUCCESS;
        };
    }

    void Start()
    {
        FadeUI.Fade?.Invoke(true);
        connectButton.interactable = false;
        connectInfoTMP.text = CONNECT_TRY;
    }

    public void OnClickConnectButton()
    {
        FadeUI.Fade?.Invoke(false);
        PhotonNetwork.LoadLevel("LobbyScene");
    }

}
