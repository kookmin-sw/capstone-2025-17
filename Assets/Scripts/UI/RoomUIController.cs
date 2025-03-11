using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Collections;

// Controller
public class RoomUIController : MonoBehaviour
{ 
    [SerializeField]
    RoomManager roomManager;

    [SerializeField]
    GameObject roomPanel;

    RoomUI roomView;
    ServerInfo roomModel;
    ClientInfo profileModel;

    void Awake()
    {
        roomView = GetComponent<RoomUI>();
        roomModel = GetComponent<ServerInfo>();
        profileModel = GetComponent<ClientInfo>();
    }

    private void Start()
    {
        // room view �̺�Ʈ ���
        roomView.randomBtn.onClick.AddListener(() => OnClickRandomBtn());
        roomView.leaveBtn.onClick.AddListener(() => OnClickLeaveBtn());

        // create view �̺�Ʈ ���
        roomView.c_confirmBtn.onClick.AddListener(() => OnClickCreateConfirmBtn());
        
        // join view �̺�Ʈ ���
        roomView.j_confirmBtn.onClick.AddListener(() => OnClickJoinConfirmBtn());

        // NetworkManager �̺�Ʈ ��� 
        NetworkManager.OnRoomPlayerEntered += RenderPlayers;
        NetworkManager.OnRoomPlayerLeaved += RemoveRenderedPlayers;
        NetworkManager.OnRoomEntered += OnEnteredRoom;
    }

    // =================== Lobby Buttons =====================
    void OnClickRandomBtn()
    {
        // ���� ��ġ ��ư Ŭ�� �� 
        SaveProfileInfo();

        roomModel.RoomType = ServerInfo.RoomTypes.Random;
        roomManager.RandomRoom();
    }

    void OnClickCreateConfirmBtn()
    {
        // �� ���� Ȯ�� ��ư Ŭ�� �� 
        SaveProfileInfo();

        string roomCode =$"{Random.Range(10000, 99999)}";
        Debug.Log(roomCode);
        int _maxPlayer = (int)roomView.maxPlayerCount.value;

        roomModel.RoomType = ServerInfo.RoomTypes.Create;

        roomManager.CreateRoom(roomCode, _maxPlayer);
    }


    void OnClickJoinConfirmBtn()
    {
        // �� �����ϱ� ��ư Ŭ�� �� 
        SaveProfileInfo();

        roomModel.RoomType = ServerInfo.RoomTypes.Join;
        string code = roomView.roomCodeTMPInp.text;

        roomManager.JoinRoom(code);
    }

    // ================== In Room ===========================

    void OnEnteredRoom()
    {
        // �� ���� ���� �� �޼ҵ� ȣ��
        roomPanel.SetActive(true);

        string roomCode = roomManager.GetRoomCode();
        roomView.roomCode.text = $"Room Code : {roomCode}";
    }

    void OnClickLeaveBtn()
    {
        // �� ������
        roomManager.LeaveRoom();
    }

    public void SaveProfileInfo()
    {
        // �뿡 ���� �� Ŭ���̾�Ʈ�� ������ �����Ѵ�
        string nickname = profileModel.Nickname;
        int characterId = profileModel.CharacterId;

        roomManager.SendClientInfo(nickname, characterId);
    }

    Dictionary<int, Hashtable> playersInfo = new Dictionary<int, System.Collections.Hashtable>();

    public void RenderPlayers()
    {
        // RoomManager���� �÷��̾� ����Ʈ�� ��û�ؼ� �޾ƿ� ��,
        // RoomUI ���� �����Ͽ� UI�� ������Ʈ ��Ų��
        playersInfo = roomManager.RenderPlayers();

        roomView.RenderPlayerUI(playersInfo);
    }

    public void RemoveRenderedPlayers(int actorNumber)
    {
        // ���� ���� �÷��̾� UI���� �����ϱ� 
        roomView.RemovePlayerUI(actorNumber);
    }

}
