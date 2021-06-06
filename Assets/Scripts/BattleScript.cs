using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleScript : MonoBehaviour
{
    public Spinner spinnerScript;

    private float startSpinSpeed;
    private float currentSpinSpeed;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatio_Text;

    private void Awake()
    {
        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //スピナートップのスピード比較
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            Debug.Log("My speed:" + mySpeed + ".....other player speed:" + otherPlayerSpeed);

            if (mySpeed > otherPlayerSpeed)
            {
                Debug.Log(" YOU DAMAGE the other player. ");

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    //スピードの遅いプレーヤーにダメージを与える
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, 400f);
                }
            }
        }
    }

    [PunRPC]
    public void DoDamage(float _damegeAmount)
    {
        spinnerScript.spinSpeed -= _damegeAmount;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
