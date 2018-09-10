using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollMovement : MonoBehaviour
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

        //apply steering
        if (currentSpeed > 10f)
        {
            frWheel.steerAngle = steerRotation * (10 / currentSpeed);
            flWheel.steerAngle = steerRotation * (10 / currentSpeed);
        }
        else
        {
            frWheel.steerAngle = steerRotation;
            flWheel.steerAngle = steerRotation;
        }
    }

    void FixedUpdate()
    {
        /*
        float wheelRadius = brWheel.radius;
        float wheelRPM = brWheel.rpm;
        float circumference; //here we will store the circumference
        float speedInMph; // and here the speed in mhiles in hour

        circumference = 2.0f * 3.14f * wheelRadius; // Finding circumFerence 2 Pi R
        speedInMph = ((circumference * wheelRPM) * 60) * 0.62f; // finding mph
        */

        //create/update acceleration values
        float acceleration = Input.GetAxis("Vertical") * motorForce;

        //store speed
        currentSpeed = rb.velocity.magnitude;

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

        //print(frWheel.steerAngle);
    }
}
