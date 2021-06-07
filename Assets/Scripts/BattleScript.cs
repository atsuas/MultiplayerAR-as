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

    public float common_Damage_Coefficient = 0.04f;

    public bool isAttaker;
    public bool isDefender;

    [Header("Player Type Damage Coefficients")]
    public float doDamage_Coefficient_Attacker = 10f; //do more damage then defender- ADVANTAGE
    public float getDamage_Coefficient_Attaker = 1.2f; //gets more damage- DISADVANTAGE

    public float doDamage_Coefficient_Defender = 0.75f; //do less damage- DISADVANTAGE
    public float getDamage_Coefficient_Defender = 0.2f; //gets less damage- ADVANTAGE

    private void Awake()
    {
        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    }

    private void CheckPlayerType()
    {
        if (gameObject.name.Contains("Attaker"))
        {
            isAttaker = true;
            isDefender = false;
        }
        else if (gameObject.name.Contains("Defender"))
        {
            isDefender = true;
            isAttaker = false;

            spinnerScript.spinSpeed = 4400;

            startSpinSpeed = spinnerScript.spinSpeed;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;
        }
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
                float default_Damage_Amount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600f * common_Damage_Coefficient;

                if (isAttaker)
                {
                    default_Damage_Amount *= doDamage_Coefficient_Attacker;
                }
                else if (isDefender)
                {
                    default_Damage_Amount *= doDamage_Coefficient_Defender;
                }

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    //スピードの遅いプレーヤーにダメージを与える
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, default_Damage_Amount);
                }
            }
        }
    }

    [PunRPC]
    public void DoDamage(float _damegeAmount)
    {
        if (isAttaker)
        {
            _damegeAmount *= getDamage_Coefficient_Attaker;
        }
        else if (isDefender)
        {
            _damegeAmount *= getDamage_Coefficient_Defender;
        }

        spinnerScript.spinSpeed -= _damegeAmount;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

    }

    void Start()
    {
        CheckPlayerType();
    }

    void Update()
    {
        
    }
}
