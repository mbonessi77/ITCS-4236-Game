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
    private float topSpeed, currentSpeed, steerSpeed;
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
        steerRotation *= 3 / (currentSpeed + 3);

        float steerAngleFR = Mathf.LerpAngle(frWheel.steerAngle, steerRotation, steerSpeed * Time.deltaTime);
        float steerAngleFL = Mathf.LerpAngle(flWheel.steerAngle, steerRotation, steerSpeed * Time.deltaTime);

        frWheel.steerAngle = steerAngleFR;
        flWheel.steerAngle = steerAngleFL;

        ApplyLocalPositionToVisuals(frWheel);
        ApplyLocalPositionToVisuals(flWheel);
        ApplyLocalPositionToVisuals(brWheel);
        ApplyLocalPositionToVisuals(blWheel);

        //display player speed in console
        //print(rb.velocity.magnitude);
    }

    void FixedUpdate()
    {
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
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        //if there is not a child object (the visual wheel)
        if (collider.transform.childCount == 0)
        {
            return;
        }

        //assign child object (the visual wheel)
        Transform visualWheel = collider.transform.GetChild(0);

        //get and store the rotation and position of the wheel collider
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        //assign those values from the wheel collider to the visual wheels
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
}
