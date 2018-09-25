using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollMovement2 : MonoBehaviour
{
    [SerializeField]
    private float motorForce, steerForce, brakeForce, slowForce;
    [SerializeField]
    private WheelCollider frWheel, flWheel, brWheel, blWheel;
    [SerializeField]
    private float topSpeed, currentSpeed;
    [SerializeField]
    private Rigidbody rb;

    void Start()
    {
        
    }

    void Update()
    {
        //create/update steering values
        float steerRotation = Input.GetAxis("Horizontal") * steerForce;

        //store speed
        currentSpeed = rb.velocity.magnitude;

        //apply steering
        if (currentSpeed > 10f)
        {
            //if going faster that 10 turning angle is reduced
            frWheel.steerAngle = steerRotation * (10 / currentSpeed);
            flWheel.steerAngle = steerRotation * (10 / currentSpeed);
        }
        else
        {
            //else going slow enough for full turning angles
            frWheel.steerAngle = steerRotation;
            flWheel.steerAngle = steerRotation;
        }
    }

    void FixedUpdate()
    {
        /*
        //For speeds??????
        float wheelRadius = brWheel.radius;
        float wheelRPM = brWheel.rpm;
        float circumference; //here we will store the circumference
        float speedInMph; // and here the speed in mhiles in hour

        circumference = 2.0f * 3.14f * wheelRadius; // Finding circumFerence 2 Pi R
        speedInMph = ((circumference * wheelRPM) * 60) * 0.62f; // finding mph
        */

        //create acceleration values
        float acceleration;

        //update acceleration values
        if (currentSpeed > (topSpeed * 0.60f))
        {
            //if car is currently going faster the 3/5 of top speed reduce the amount of motor force added
            acceleration = Input.GetAxis("Vertical") * (motorForce * 0.5f);
        }
        else
        {
            //else car is going less than 3/5 of top speed allow full amount of motor force to be added
            acceleration = Input.GetAxis("Vertical") * motorForce;//JUST THIS IF WANT TO REVERT TO PREVIOUS VERSION
        }

        //apply acceleration
        if (currentSpeed < topSpeed)
        {
            brWheel.motorTorque = acceleration;
            blWheel.motorTorque = acceleration;
        }
        else
        {
            brWheel.motorTorque = 0;
            blWheel.motorTorque = 0;
        }

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

        print(acceleration);
    }
}
