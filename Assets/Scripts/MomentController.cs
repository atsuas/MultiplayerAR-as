using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentController : MonoBehaviour
{
    public Joystick joystick;
    public float speed = 4;
    private Vector3 velocityVector = Vector3.zero; //初速度

    public float maxVelocityChange = 4f;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Joystickの入力を取る
        float _xMovementInput = joystick.Horizontal;
        float _zMovementInput = joystick.Vertical;

        //速さのベクトルの計算
        Vector3 _movementHorizontal = transform.right * _xMovementInput;
        Vector3 _movementVertical = transform.forward * _zMovementInput;

        //最終的な移動速度を計算する
        Vector3 _movementVelocityVector = (_movementHorizontal + _movementVertical).normalized * speed;

        //Movementの適用
        Move(_movementVelocityVector);
    }

    void Move(Vector3 movementVelocicityVector)
    {
        velocityVector = movementVelocicityVector;
    }

    private void FixedUpdate()
    {
        if (velocityVector != Vector3.zero)
        {
            //Rigidbodyの現在の速度を得る
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = velocityVector - velocity;

            //目標速度に到達するまでの速度変化分の力を加える
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0f;

            rb.AddForce(velocityChange, ForceMode.Acceleration);
        }
    }
}
