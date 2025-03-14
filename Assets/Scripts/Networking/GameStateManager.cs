using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameStateManager : MonoBehaviourPun
{
    public static GameStateManager Instance { get; private set; }
    
    //게임 시작 상태
    private bool isGameStarted = false;
    
    //싱글톤
    private void Awake()
    {
        if(Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
    //게임 시작 RPC
    [PunRPC]
    public void StartGame()
    {
        isGameStarted = true;
        photonView.RPC("SyncGameState", RpcTarget.All, isGameStarted);
    }
    
    //게임 종료 RPC
    [PunRPC]
    public void EndGame()
    {
        isGameStarted = false;
        photonView.RPC("SyncGameState", RpcTarget.All, isGameStarted);
    }
    
    //게임 상태 동기화 RPC
    [PunRPC]
    public void SyncGameState(bool state)
    {
        isGameStarted = state;
    }
    
    //게임 종료 조건 체크
    public void CheckGameEnd()
    {
        if (!isGameStarted) return;
        
        //모든 미션을 성공했다면
        if (MissionManager.Instance.AreAllMissionsComplete())
        {
            isGameStarted = false;
            
            //게임 클리어 연출 씬 실행
            photonView.RPC("GameClear", RpcTarget.All);
            return; // 게임 클리어 상태면 게임 오버 체크하지 않음
        }
        //제한 시간이 초과되었는지 확인
        if (isGameStarted && GameTimer.Instance.IsTimeOver())
        {
            isGameStarted = false;
            photonView.RPC("GameOver", RpcTarget.All);
        }
    }
    
    [PunRPC]
    private void GameClear()
    {
        PhotonNetwork.LoadLevel("GameClearScene");
    }

    [PunRPC]
    private void GameOver()
    {
        PhotonNetwork.LoadLevel("GameOverScene");
    }
}
