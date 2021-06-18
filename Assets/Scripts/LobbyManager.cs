using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInputField;
    public GameObject uI_LoginGameobject;

    [Header("Lobby UI")]
    public GameObject uI_LobbyGameobject;
    public GameObject uI_3DGameobject;

    [Header("Connection Status UI")]
    public GameObject uI_ConnectionStatusGameobject;
    public Text connectionStatusText;
    public bool showConnectionStatus = false;


    #region Unity Methods
    void Start()
    {
        //Lobby UIだけを起動する
        if (PhotonNetwork.IsConnected)
        {
            uI_LobbyGameobject.SetActive(true);
            uI_3DGameobject.SetActive(true);
            uI_ConnectionStatusGameobject.SetActive(false);

            uI_LoginGameobject.SetActive(false);
        }
        else
        {
            //まだPhotonに接続していないので、Login UIのみ起動している
            uI_LobbyGameobject.SetActive(false);
            uI_3DGameobject.SetActive(false);
            uI_ConnectionStatusGameobject.SetActive(false);

            uI_LoginGameobject.SetActive(true);
        }

    }

    void Update()
    {
        if (showConnectionStatus)
        {
            connectionStatusText.text = "接続状況: " + PhotonNetwork.NetworkClientState;
        }
        
    }

    #endregion



    #region UI Callback Methods
    public void OnEnterGameButtonClicked()
    {
        string playerName = playerNameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            uI_LobbyGameobject.SetActive(false);
            uI_3DGameobject.SetActive(false);
            uI_LoginGameobject.SetActive(false);

            showConnectionStatus = true;
            uI_ConnectionStatusGameobject.SetActive(true);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;

                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("プレイヤーの名前が無効、または空になっています");
        }
    }

    public void OnQuickMatchButtonClicked()
    {
        //SceneManager.LoadScene("Scene_Loading");
        Scene_Loader.Instance.LoadScene("Scene_PlayerSelection");
    }

    #endregion


    #region PHOTON Callback Method
    public override void OnConnected()
    {
        Debug.Log("インターネットに接続しました");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " がPhotonサーバーに接続中");

        uI_LobbyGameobject.SetActive(true);
        uI_3DGameobject.SetActive(true);

        uI_LoginGameobject.SetActive(false);
        uI_ConnectionStatusGameobject.SetActive(false);
    }
    #endregion

}
