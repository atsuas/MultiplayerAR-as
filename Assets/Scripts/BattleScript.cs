using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleScript : MonoBehaviourPun
{
    public Spinner spinnerScript;

    public GameObject uI_3D_Gameobject;
    public GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameobject;

    private Rigidbody rb;

    private float startSpinSpeed;
    private float currentSpinSpeed;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatio_Text;

    public float common_Damage_Coefficient = 0.04f;

    public bool isAttaker;
    public bool isDefender;
    private bool isDead = false;

    [Header("Player Type Damage Coefficients")]
    public float doDamage_Coefficient_Attacker = 10f; //ディフェンダータイプよりも大きなダメージを与える- ADVANTAGE
    public float getDamage_Coefficient_Attaker = 1.2f; //ダメージが大きくなる- DISADVANTAGE

    public float doDamage_Coefficient_Defender = 0.75f; //ダメージが少ない- DISADVANTAGE
    public float getDamage_Coefficient_Defender = 0.2f; //ダメージが少なくなる- ADVANTAGE

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
            if(photonView.IsMine)
            {
                Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3(0, 0.05f, 0);

                //Instantiate Collision Effect ParticleSystem
                GameObject collisionEffectGameobject = GetPooledObject();
                if (collisionEffectGameobject != null)
                {
                    collisionEffectGameobject.transform.position = effectPosition;
                    collisionEffectGameobject.SetActive(true);
                    collisionEffectGameobject.GetComponentInChildren<ParticleSystem>().Play();

                    //De-activate Collision Effect Particle System after some seconds.
                    StartCoroutine(DeactivateAfterSeconds(collisionEffectGameobject, 0.5f));

                }
            }

            //スピナートップのスピード比較
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            Debug.Log("スピード:" + mySpeed + ".....他のプレイヤーのスピード:" + otherPlayerSpeed);

            if (mySpeed > otherPlayerSpeed)
            {
                Debug.Log(" 相手のプレイヤーにダメージを与える ");
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
        if (!isDead)
        {
            if (isAttaker)
            {
                _damegeAmount *= getDamage_Coefficient_Attaker;

                // 追加部分
                if (_damegeAmount > 1000)
                {
                    _damegeAmount = 400f;
                }
            }
            else if (isDefender)
            {
                _damegeAmount *= getDamage_Coefficient_Defender;
            }

            spinnerScript.spinSpeed -= _damegeAmount;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

            if (currentSpinSpeed < 100)
            {
                //死ぬ
                Die();
            }
        }
    }

    void Die()
    {
        isDead = true;

        GetComponent<MomentController>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        spinnerScript.spinSpeed = 0f;

        uI_3D_Gameobject.SetActive(false);


        if (photonView.IsMine)
        {
            //リスポーンまでのカウントダウン
            StartCoroutine(ReSpawn());
        }
    }

    IEnumerator ReSpawn()
    {
        GameObject canvasGameobject = GameObject.Find("Canvas");
        if (deathPanelUIGameobject==null)
        {
            deathPanelUIGameobject = Instantiate(deathPanelUIPrefab, canvasGameobject.transform);
        }
        else
        {
            deathPanelUIGameobject.SetActive(true);
        }

        Text respawnTimeText = deathPanelUIGameobject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;

        respawnTimeText.text = respawnTime.ToString(".00");

        while(respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");

            GetComponent<MomentController>().enabled = false;

        }

        deathPanelUIGameobject.SetActive(false);

        GetComponent<MomentController>().enabled = true;

        photonView.RPC("Reborn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void Reborn()
    {
        spinnerScript.spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        uI_3D_Gameobject.SetActive(true);

        isDead = false;
    }

    //void Start()
    //{
    //    CheckPlayerType();

    //    rb = GetComponent<Rigidbody>();
    //}

    //void Update()
    //{

    //}


    public List<GameObject> pooledObjects;
    public int amountToPool = 8;
    public GameObject CollisionEffectPrefab;
    // Start is called before the first frame update
    void Start()
    {
        CheckPlayerType();

        rb = GetComponent<Rigidbody>();


        if (photonView.IsMine)
        {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(CollisionEffectPrefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }



    }

    public GameObject GetPooledObject()
    {

        for (int i = 0; i < pooledObjects.Count; i++)
        {

            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);

    }
}
