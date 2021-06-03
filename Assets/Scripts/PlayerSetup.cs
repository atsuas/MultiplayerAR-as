using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPun
{
    
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
    }

    void Update()
    {
        
    }
}
