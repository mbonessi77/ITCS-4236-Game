using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollMovement : MonoBehaviour
{
    [SerializeField]
    private float motorForce, steerForce, brakeForce, slowForce;
    [SerializeField]
    private WheelCollider frWheel, flWheel, brWheel, blWheel;

    void Start()
    {

    }

    void Update()
    {
        //create/update acceleration and steering values
        float acceleration = Input.GetAxis("Vertical") * motorForce;
        float steerRotation = Input.GetAxis("Horizontal") * steerForce;

        //apply acceleration
        brWheel.motorTorque = acceleration;
        blWheel.motorTorque = acceleration;

        //apply steering
        frWheel.steerAngle = steerRotation;
        flWheel.steerAngle = steerRotation;

        //user applied brakes
        if (Input.GetKey(KeyCode.Space))
        {
            brWheel.brakeTorque = brakeForce;
            blWheel.brakeTorque = brakeForce;
        }

        //car's natural slow without acceleration applied
        if(Input.GetAxis("Vertical") == 0)
        {
            brWheel.brakeTorque = slowForce;
            blWheel.brakeTorque = slowForce;
        }
        else
        {
            brWheel.brakeTorque = 0;
            blWheel.brakeTorque = 0;
        }
    }
}
