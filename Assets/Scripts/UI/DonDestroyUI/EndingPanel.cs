using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingPanel : MonoBehaviour
{
    PopupUI popup;

    private void Start()
    {
        popup = GetComponent<PopupUI>();
    }

    public void OnClickOkBtn()
    {
        Managers.GameStateManager.RPC_LeaveRoomAllPlayer();
        PhotonNetwork.LoadLevel("LobbyScene");
    }


}
