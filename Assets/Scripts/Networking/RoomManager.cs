using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

// Room �� ���õ� ��Ʈ��ũ �۾��� ����ϴ� Ŭ����
public class RoomManager : MonoBehaviour
{
    public static Dictionary<int, Player> s_players;

    // �ؽ� Ű
    const string NicknameKey = "Nickname";
    const string CharacterIdKey = "CharacterId";

    // ���� ��ġ�� ��û�Ѵ� 
    public void RandomRoom()
    {
        Debug.Log("Join Or Create Room()");

        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 4;

        PhotonNetwork.JoinOrCreateRoom(
            "Random",
            room,
            TypedLobby.Default
            );
    }

    // �� ������ ��û�Ѵ� 
    public void CreateRoom(string roomName, int maxPlayer = 4)
    {
        RoomOptions room = new RoomOptions();
        room.MaxPlayers = maxPlayer;

        PhotonNetwork.CreateRoom
       (
            roomName,
            room,
            TypedLobby.Default
       );
    }

    // �� ������ ��û�Ѵ� 
    public void JoinRoom(string code)
    {
        PhotonNetwork.JoinRoom
       (
            code
       );
    }

    // ���� ���´� 
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        s_players.Clear();
        RenderPlayers();
    }

    // Ŭ���̾�Ʈ�� ������ �����Ѵ� 
    bool isFirstSend = true;

    public void SendClientInfo(string nickname, int characterId)
    {
        Hashtable hash = new Hashtable();
        if (isFirstSend)
        {
            // ó������ ������ ������ ���
            isFirstSend = false;

            hash.Add(NicknameKey, nickname);
            hash.Add(CharacterIdKey, characterId);
        }
        else
        {
            // �̹� ������ ������ ���
            hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash[NicknameKey] = nickname;
            hash[CharacterIdKey] = characterId;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    // ���濡�� �÷��̾� ǥ�ø� ���� ���� ���� �÷��̾� ������ �޾ƿ´�
    public Dictionary<int, System.Collections.Hashtable> RenderPlayers()
    {
        s_players = PhotonNetwork.CurrentRoom.Players;

        // Player�� ActorNumber�� ������ ���� HashTable 
        Dictionary<int, System.Collections.Hashtable> playersInfo = new Dictionary<int, System.Collections.Hashtable>();

        foreach(KeyValuePair<int, Player> p in s_players)
        {
            Hashtable hash = p.Value.CustomProperties;
            System.Collections.Hashtable newHash = new System.Collections.Hashtable();

            if (hash != null)
            {
                string nickname = (string)hash[NicknameKey];
                int characterId = (int)hash[CharacterIdKey];

                newHash.Add(NicknameKey, nickname);
                newHash.Add(CharacterIdKey , characterId);

                playersInfo.Add(p.Key, newHash);
            }
            else
            {
                // hash�� ����ִ� ��� �ӽ÷� �⺻ �� ����
                newHash.Add(NicknameKey, $"USER_{Random.Range(100, 999)}");
                newHash.Add(CharacterIdKey, 0);

                playersInfo.Add(p.Key, newHash);
            }
        }
        return playersInfo;
    }

    // ���� ���� �� �ڵ带 �޾ƿ´�
    public string GetRoomCode()
    {
        string roomCode = PhotonNetwork.CurrentRoom.Name;
        return roomCode;
    }

}
