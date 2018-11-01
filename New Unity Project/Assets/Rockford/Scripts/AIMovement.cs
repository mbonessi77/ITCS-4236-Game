using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    /// <summary>
    /// AI car movement:
    ///     AI will take target position - current position (normalized) as "Horizontal" and "Vertical" inputs
    ///     May need radius of satisfaction for rotational movement to stop snake movement
    /// </summary>

    [SerializeField]
    private float motorForce, steerForce, brakeForce, slowForce, radiusOfSat;
    [SerializeField]
    private WheelCollider frWheel, flWheel, brWheel, blWheel;
    [SerializeField]
    private float topSpeed, currentSpeed, steerSpeed;
    [SerializeField]
    private Rigidbody rb;
    private Vector3 inputVector;
    private float rotDirLR;
    private float rotDirFB;
    [SerializeField]
    private Vector3 targetPos;

    void Start()
    {

    }

    void Update()
    {
        //For AI, inputVector should be target location - current location instead of Horizontal and Vertical Axis
        inputVector = targetPos - transform.position;
        inputVector.y = transform.position.y;
        inputVector.Normalize();

        rotDirLR = Vector3.Dot(transform.right, inputVector);
        rotDirFB = Vector3.Dot(transform.forward, inputVector);
        float steerRotation;
        //create/update steering values
        //if target is behind the AI car
        if (rotDirFB < 0)
        {
            //if target is behind and to the right of the AI car
            if (rotDirLR > 0)
            {
                steerRotation = -1 * steerForce;
            }
            else
            {
                //else target is behind and to the right of the AI car
                steerRotation = 1 * steerForce;
            }
        }
        else
        {
            //else target is in front of the AI car
            //if target in front and to the right of the AI car
            if (rotDirLR > 0)
            {
                steerRotation = rotDirLR * steerForce;
            }
            else
            {
                //else target is in front and to the left of the AI car
                steerRotation = rotDirLR * steerForce;
            }
        }

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

        //display AI speed in console
        //print(rb.velocity.magnitude);
        //print("DotLR: " + rotDirLR + " DotFB: " + rotDirFB);

        Debug.DrawLine(targetPos + new Vector3(0f, 10f, 0f), targetPos, Color.red);
    }

    void FixedUpdate()
    {
        //create acceleration values
        float acceleration;

        //update acceleration values
        //if target is behind the AI car
        if (rotDirFB < 0)
        {
            //fraction of motor force when going forward
            acceleration = -1 * (motorForce * 0.25f);
        }
        else
        {
            //else target is in front of the AI car
            if (currentSpeed > (topSpeed * 0.60f))
            {
                //if car is currently going faster the 3/5 of top speed reduce the amount of motor force added
                acceleration = 1 * (motorForce * 0.5f);
            }
            else
            {
                //else car is going less than 3/5 of top speed allow full amount of motor force to be added
                acceleration = 1 * motorForce;//JUST THIS IF WANT TO REVERT TO PREVIOUS VERSION
            }
        }

        //apply acceleration
        //if not at top speed and not in radius of satisfaction of target position
        if (currentSpeed < topSpeed && Vector3.Distance(targetPos, transform.position) > radiusOfSat)
        {
            brWheel.motorTorque = acceleration;
            blWheel.motorTorque = acceleration;
        }
        else
        {
            //else at top speed or in radius of satisfaction of target position
            brWheel.motorTorque = 0;
            blWheel.motorTorque = 0;
        }

        //apply brakes
        //if within the radius of satisfaction of the target position
        if (Vector3.Distance(targetPos, transform.position) < radiusOfSat)
        {
            brWheel.brakeTorque = brakeForce;
            blWheel.brakeTorque = brakeForce;
        }
        else
        {
            //else not within radius of satisfaction
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
