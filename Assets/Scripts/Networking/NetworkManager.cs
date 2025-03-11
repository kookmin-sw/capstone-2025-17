using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum ConnectState
{
    Idle,
    Lobby,
    Room,
    InGame,
    Disconnected
}

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // �÷��̾��� ���� ����
    static ConnectState sConnectState;

    static string _gameVersion = "1";

    public static Action OnConnectedToServer;
    public static Action OnRoomPlayerEntered;
    public static Action OnRoomEntered;
    public static Action OnRequestFailed;

    // =================== int�� �����ؾ� �Ѵ� : ActorNumber ��� ===============
    public static Action<string> OnRoomPlayerLeaved;


    void Start()
    {
        sConnectState = ConnectState.Idle;

        DontDestroyOnLoad(gameObject);

        ConnectToMasterServer();
    }

    public static void ConnectToMasterServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            // �� ����ȭ
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = _gameVersion;

            // ���� �õ�
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    #region Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("NetworkManager.cs - On Connected To Master()");

        // ������ ���� ���� ���� ��, �ٷ� �κ� ���� �õ�
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("NetworkManager.cs - On Disconnected()");

        // ���� ���� ��, ������ �õ� 
        int retry = 0,  maxRetry = 5;
        if (retry < maxRetry)
        {
            retry++;

            Debug.Log("NetworkManager.cs - retry connect to master server");

            Invoke(nameof(ConnectToMasterServer), 2f);
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("On Joined Lobby()");

        sConnectState = ConnectState.Lobby;

        // StartSceneUI.cs ���� ��ϵ� �̺�Ʈ
        OnConnectedToServer?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        // �濡 ���������� ���� ��, �÷��̾� ������ ������ �����ϱ�
        if (PhotonNetwork.IsConnected)
        {
            sConnectState = ConnectState.Room;

            // �̺�Ʈ�� �����Ѵ� 
            OnRoomEntered?.Invoke();
            OnRoomPlayerEntered?.Invoke();
        }
        else
        {
            // ���� ���� ó��
        }
        
    }
    public override void OnLeftRoom()
    {
        Debug.Log("On Left Room()");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // �� ���� ���� ó��
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("NetworkManager.cs - On Created Room()");
        Debug.Log($"NetworkManager.cs - {PhotonNetwork.CurrentRoom.Name}");
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnRoomPlayerEntered?.Invoke();
        Debug.Log("Entered Player");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        string nickname = (string)otherPlayer.CustomProperties["Nickname"];
        Debug.Log("NetworkManager - ���� ���� : " + nickname);
        OnRoomPlayerLeaved?.Invoke(nickname);
    }
    #endregion

}
