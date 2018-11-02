using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollMovement : MonoBehaviour
{
    /// <summary>
    /// User car movement:
    ///     User uses "Horizontal" and "Vertical" as inputs for movement
    /// </summary>

    [SerializeField]
    private float motorForce, steerForce, brakeForce;
    [SerializeField]
    private WheelCollider frWheel, flWheel, brWheel, blWheel;
    [SerializeField]
    private float topSpeed, currentSpeed, steerSpeed;
    [SerializeField]
    private Rigidbody rb;
    private Vector3 localVel;
    private Vector3 inputVector;

    void Start()
    {
        
    }

    void Update()
    {
        //For AI, inputVector should be target location - current location instead of Horizontal and Vertical Axis
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        //store local velocity
        localVel = transform.InverseTransformDirection(rb.velocity);

        //create/update steering values
        float steerRotation = inputVector.x * steerForce;

        //store speed
        currentSpeed = rb.velocity.magnitude;

        //create desired steer rotation
        steerRotation *= 3 / (currentSpeed + 3);

        //lerp wheel rotation so wheels don't just instantly assign to new direction
        float steerAngleFR = Mathf.LerpAngle(frWheel.steerAngle, steerRotation, steerSpeed * Time.deltaTime);
        float steerAngleFL = Mathf.LerpAngle(flWheel.steerAngle, steerRotation, steerSpeed * Time.deltaTime);

        //apply steering (of the lerping rotation)
        frWheel.steerAngle = steerAngleFR;
        flWheel.steerAngle = steerAngleFL;

        //apply rotation to all visual wheels (steering and rpm)
        ApplyLocalPositionToVisuals(frWheel);
        ApplyLocalPositionToVisuals(flWheel);
        ApplyLocalPositionToVisuals(brWheel);
        ApplyLocalPositionToVisuals(blWheel);

        //display player speed in console
        //print(currentSpeed);
        //print(localVel.z);
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
        //if not at top speed and acceleration is being pressed
        if (currentSpeed < topSpeed && Mathf.Abs(inputVector.z) > 0.1f)
        {
            brWheel.motorTorque = acceleration;
            blWheel.motorTorque = acceleration;
        }
        else
        {
            //else acceleration is not being pressed, OR at top speed so slow acceleration to limit car to top speed
            brWheel.motorTorque = 0;
            blWheel.motorTorque = 0;
        }

        //apply brakes/deceleration
        if (Input.GetKey(KeyCode.Space))
        {
            //car's natural deceleration
            rb.velocity *= 0.997f;
            //apply actual brakes to wheel colliders for added deceleration
            brWheel.brakeTorque = brakeForce;
            blWheel.brakeTorque = brakeForce;
            frWheel.brakeTorque = brakeForce;
            flWheel.brakeTorque = brakeForce;
        }
        else if(inputVector.z == 0)
        {
            //if not pressing acceleration
            //car's natural deceleration only
            rb.velocity *= 0.997f;
            //no added brakes to wheel colliders
            brWheel.brakeTorque = 0;
            blWheel.brakeTorque = 0;
            frWheel.brakeTorque = 0;
            flWheel.brakeTorque = 0;
        }
        else
        {
            //else no brakes or deceleration
            brWheel.brakeTorque = 0;
            blWheel.brakeTorque = 0;
            frWheel.brakeTorque = 0;
            flWheel.brakeTorque = 0;
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
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
