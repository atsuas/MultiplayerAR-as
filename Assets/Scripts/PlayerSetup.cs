using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public TextMeshProUGUI playerNameText;
    
    void Start()
    {
        if (photonView.IsMine)
        {
            //プレイヤーはローカルプレイヤー
            transform.GetComponent<MomentController>().enabled = true;
            transform.GetComponent<MomentController>().joystick.gameObject.SetActive(true);
        }
        else
        {
            //プレイヤーはリモートプレイヤー
            transform.GetComponent<MomentController>().enabled = false;
            transform.GetComponent<MomentController>().joystick.gameObject.SetActive(false);
        }
        SetPlayerName();
    }

    void SetPlayerName()
    {
        if (playerNameText != null)
        {
            if (photonView.IsMine)
            {
                playerNameText.text = "YOU";
                playerNameText.color = Color.red;
            }
            else
            {
                playerNameText.text = photonView.Owner.NickName;
            }
        }
    }
}
