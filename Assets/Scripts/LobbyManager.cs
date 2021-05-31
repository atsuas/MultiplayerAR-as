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
        //Activeting only Lobby UI
        if (PhotonNetwork.IsConnected)
        {
            uI_LobbyGameobject.SetActive(true);
            uI_3DGameobject.SetActive(true);
            uI_ConnectionStatusGameobject.SetActive(false);

            uI_LoginGameobject.SetActive(false);
        }
        else
        {
            //Activating only Login UI since we did noy connect to photon yet.
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
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
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
            Debug.Log("Player name is invalid or enpty!");
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
        Debug.Log("We connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon Server");

        uI_LobbyGameobject.SetActive(true);
        uI_3DGameobject.SetActive(true);

        uI_LoginGameobject.SetActive(false);
        uI_ConnectionStatusGameobject.SetActive(false);
    }
    #endregion

}
