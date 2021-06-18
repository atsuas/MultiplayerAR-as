using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class SpinningTopsGameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject uI_InformmPanelGameobject;
    public TextMeshProUGUI uI_InformText;
    public GameObject searchForGamesBottonGameobject;
    public GameObject adjust_Button;
    public GameObject raycastCenter_Image;

    void Start()
    {
        uI_InformmPanelGameobject.SetActive(true);
        //uI_InformText.text = "Serch For Games to BATTLE!";
    }

    #region UI Callback Method
    public void JoinRandomRoom()
    {
        uI_InformText.text = "ルームを検索しています...";

        PhotonNetwork.JoinRandomRoom();

        searchForGamesBottonGameobject.SetActive(false);
    }

    public void OnQuitMatchButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Scene_Loader.Instance.LoadScene("Scene_Lobby");
        }
    }
    #endregion


    #region PHOTON Callback Method
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        uI_InformText.text = message;

        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        adjust_Button.SetActive(false);
        raycastCenter_Image.SetActive(false);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = " 　参加しました! " + PhotonNetwork.CurrentRoom.Name + " \n他のプレイヤーを待っています...";
        }
        else
        {
            uI_InformText.text = " 参加しました! " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(uI_InformmPanelGameobject, 2.0f));
        }

        Debug.Log(" 参加しました! " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " 参加しました! " + PhotonNetwork.CurrentRoom.Name + " プレイヤー数: " + PhotonNetwork.CurrentRoom.PlayerCount);

        uI_InformText.text = newPlayer.NickName + " 参加しました! " + PhotonNetwork.CurrentRoom.Name + " プレイヤー数: " + PhotonNetwork.CurrentRoom.PlayerCount;

        StartCoroutine(DeactivateAfterSeconds(uI_InformmPanelGameobject, 2.0f));
    }

    public override void OnLeftRoom()
    {
        Scene_Loader.Instance.LoadScene("Scene_Lobby");
    }
    #endregion


    #region PRIVATE Methods
    void CreateAndJoinRoom()
    {
        string randomRoomName = "ルーム:" + Random.Range(0, 1000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        //ルームを作る
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameobject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameobject.SetActive(false);
    }
    #endregion
}
