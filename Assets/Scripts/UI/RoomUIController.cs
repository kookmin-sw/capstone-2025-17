using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

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
        // room view 이벤트 등록
        roomView.randomBtn.onClick.AddListener(() => OnClickRandomBtn());
        roomView.leaveBtn.onClick.AddListener(() => OnClickLeaveBtn());

        // create view 이벤트 등록
        roomView.c_confirmBtn.onClick.AddListener(() => OnClickCreateConfirmBtn());
        
        // join view 이벤트 등록
        roomView.j_confirmBtn.onClick.AddListener(() => OnClickJoinConfirmBtn());

        // Game Lanucher 이벤트 등록 
        NetworkManager.OnRoomPlayerEntered += RenderPlayers;
        NetworkManager.OnRoomPlayerLeaved += RemoveRenderedPlayers;
        NetworkManager.OnRoomEntered += OnEnteredRoom;
    }

    // =================== Lobby Buttons =====================
    void OnClickRandomBtn()
    {
        SaveProfileInfo();

        roomModel.RoomType = ServerInfo.RoomTypes.Random;
        roomManager.RandomRoom();
    }

    void OnClickCreateConfirmBtn()
    {
        SaveProfileInfo();

        string roomCode =$"{Random.Range(10000, 99999)}";
        Debug.Log(roomCode);
        int _maxPlayer = (int)roomView.maxPlayerCount.value;

        roomModel.RoomType = ServerInfo.RoomTypes.Create;

        roomManager.CreateRoom(roomCode, _maxPlayer);
    }


    void OnClickJoinConfirmBtn()
    {
        SaveProfileInfo();

        roomModel.RoomType = ServerInfo.RoomTypes.Join;
        string code = roomView.roomCodeTMPInp.text;

        roomManager.JoinRoom(code);
    }

    // ================== In Room ===========================

    void OnEnteredRoom()
    {
        // 룸 접속 성공 시 메소드 호출
        roomPanel.SetActive(true);

        string roomCode = roomManager.GetRoomCode();
        roomView.roomCode.text = $"Room Code : {roomCode}";
    }

    void OnClickLeaveBtn()
    {
        // 룸 나가기
        roomManager.LeaveRoom();
    }

    public void SaveProfileInfo()
    {
        string nickname = profileModel.Nickname;
        int characterId = profileModel.CharacterId;

        roomManager.SendClientInfo(nickname, characterId);
    }

    Dictionary<string, int> playersInfo = new Dictionary<string, int>();

    public void RenderPlayers()
    { 
        playersInfo = roomManager.RenderPlayers();

        roomView.RenderPlayerUI(playersInfo);
    }

    public void RemoveRenderedPlayers(string nickname)
    {

        roomView.RemovePlayerUI(nickname);
    }

}
