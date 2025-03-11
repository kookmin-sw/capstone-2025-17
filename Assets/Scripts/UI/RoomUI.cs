using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    // lobby 
    public Button randomBtn;

    // room panel
    public Button leaveBtn;
    public TextMeshProUGUI roomCode;
    public GameObject[] playersUI;
    public TextMeshProUGUI[] nicknamesUI;
    public GameObject[] playersRawImage;

    // create panel
    public Slider maxPlayerCount;
    public Button c_confirmBtn;
    public Button c_cancelBtn;

    // joine panel
    public TMP_InputField roomCodeTMPInp;
    public Button j_confirmBtn;
    public Button j_cancelBtn;

    // ĳ���� ���� �޽ð� ����� Scriptable Object ���� 
    [SerializeField]
    MaterialStorage storage;

    static Dictionary<int, Hashtable> viewPlayerList;


    // ��� �濡�� �÷��̾ ǥ���Ѵ� 
    public void RenderPlayerUI(Dictionary<int, Hashtable> players)
    {

        viewPlayerList = players;

        int playerIdx = 0;

        foreach(KeyValuePair<int, Hashtable> kvp in players)
        {
            Debug.Log($"{playerIdx} ��° �÷��̾��� ���� ���̵� : {kvp.Key}");

            playersUI[playerIdx].SetActive(true);
            playersRawImage[playerIdx].SetActive(true);

            SkinnedMeshRenderer sm = playersUI[playerIdx].GetComponentInChildren<SkinnedMeshRenderer>();

            int characterId = (int) kvp.Value["CharacterId"];
            string nickname = (string)kvp.Value["Nickname"];

            sm.material = storage.GetMesh(characterId);
            nicknamesUI[playerIdx].text = nickname;

            playerIdx++;
        }
    }

    // ��� ���� ���� �÷��̾ UI���� �����Ѵ�

    public void RemovePlayerUI(int actorNumber)
    {
        int playerIdx = 0;

        foreach (KeyValuePair<int, Hashtable> p in viewPlayerList)
        {
            if (p.Key == actorNumber)
            {
                playersUI[playerIdx].SetActive (false);
                nicknamesUI[playerIdx].text = string.Empty;
                playersRawImage[playerIdx].SetActive(false);

                viewPlayerList.Remove(p.Key);

                return;
            }
            playerIdx++;
        }

    }

}
