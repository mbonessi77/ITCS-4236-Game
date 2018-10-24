using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollMovement : MonoBehaviour
{
    /// <summary>
    /// User car movement:
    ///     User uses "Horizontal" and "Vertical" as inputs for movement
    /// 
    /// AI car movement:
    ///     AI will take target position - current position (normalized) as "Horizontal" and "Vertical" inputs
    ///     May need radius of satisfaction for rotational movement to stop snake movement
    /// </summary>

    [SerializeField]
    private float motorForce, steerForce, brakeForce, slowForce;
    [SerializeField]
    private WheelCollider frWheel, flWheel, brWheel, blWheel;
    [SerializeField]
    private float topSpeed, currentSpeed;
    [SerializeField]
    private Rigidbody rb;
    private Vector3 inputVector;

    void Start()
    {
        
    }

    void Update()
    {
        //For AI, inputVector should be target location - current location instead of Horizontal and Vertical Axis
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));


        //create/update steering values
        float steerRotation = inputVector.x * steerForce;

        //store speed
        currentSpeed = rb.velocity.magnitude;

        //apply steering
/*
        if (currentSpeed > (topSpeed * 0.1f))
        {
*/
            //if going faster than ___ top speed (_), turning angle is reduced based on speed
            frWheel.steerAngle = steerRotation * (3 / (currentSpeed + 3));
            flWheel.steerAngle = steerRotation * (3 / (currentSpeed + 3));
/*
        }
        else
        {
            //else going slow enough for full turning angles
            frWheel.steerAngle = steerRotation;
            flWheel.steerAngle = steerRotation;
        }
*/

        print(rb.velocity.magnitude);
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
            acceleration = inputVector.z * (motorForce * 0.5f);
        }
        else
        {
            //else car is going less than 3/5 of top speed allow full amount of motor force to be added
            acceleration = inputVector.z * motorForce;//JUST THIS IF WANT TO REVERT TO PREVIOUS VERSION
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
        if(inputVector.z == 0)
        {
            brWheel.brakeTorque = slowForce;
            blWheel.brakeTorque = slowForce;
        }
        else
        {
            brWheel.brakeTorque = 0;
            blWheel.brakeTorque = 0;
        }

        ApplyLocalPositionToVisuals(frWheel);
        ApplyLocalPositionToVisuals(flWheel);
        ApplyLocalPositionToVisuals(brWheel);
        ApplyLocalPositionToVisuals(blWheel);
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
}
