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
    private float motorForce, steerForce, brakeForce, radiusOfSat;
    [SerializeField]
    private WheelCollider frWheel, flWheel, brWheel, blWheel;
    [SerializeField]
    private float topSpeed, currentSpeed, steerSpeed;
    [SerializeField]
    private Rigidbody rb;
    private Vector3 localVel;
    private Vector3 inputVector;
    private float rotDirLR;
    private float rotDirFB;
    private Node nextTarget;
    [SerializeField]
    private Vector3 targetPos;
    private Stack<Node> bestPath = new Stack<Node>();

    //a* script
    [SerializeField]
    private AStarSearch aStar;

    private int waypoint_int = 0;

    void Start()
    {
        //Start with the target position being where the car is
        nextTarget = new Node(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), 0);
        targetPos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));

        //----------For testing pathfinding and pathfollowing
        Invoke("CalculatePathTestFunction", 2f);
        //----------
    }

    void CalculatePathTestFunction()
    {
        //bestPath = new Stack<Node>(new Stack<Node>(aStar.CalulatePath(500, 25)));
    }

    void Update()
    {
        //if there is another node postion in the stack and car is within the radius of satisfaction, pop the next node off and set as next target
        if (bestPath.Count > 0 && Vector3.Distance(transform.position, targetPos) < radiusOfSat)
        {
            nextTarget = bestPath.Pop();
            targetPos = new Vector3(nextTarget.GetPosX(), transform.position.y, nextTarget.GetPosZ());
            //print(nextTarget.ToString());
        }
        else if (bestPath.Count <= 0)
        {
            Transform waypoint = Waypoint_Cache.waypoints[waypoint_int].transform.GetChild(Random.Range(0, 5));
            bestPath = new Stack<Node>(new Stack<Node>(aStar.CalulatePath(waypoint.position.x, waypoint.position.z)));
            waypoint_int += 1;
        }

        //For AI, inputVector should be target location - current location instead of Horizontal and Vertical Axis
        inputVector = targetPos - transform.position;
        inputVector.y = transform.position.y;
        inputVector.Normalize();

        //store local velocity
        localVel = transform.InverseTransformDirection(rb.velocity);

        //get dot products for left and right of car, >0 is right <0 left
        rotDirLR = Vector3.Dot(transform.right, inputVector);
        //get dot product for in front and behind of car, >0 is in front <0 is behind
        rotDirFB = Vector3.Dot(transform.forward, inputVector);

        //create/update steering values
        #region
        float steerRotation;
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
        #endregion

        //store speed
        currentSpeed = rb.velocity.magnitude;

        //------------ Delete (here as backup, in case I need to revert to previous version)
        //create desired steer rotation
        //steerRotation *= 3 / (currentSpeed + 3);
        //------------

        //lerp wheel rotation so wheels don't just instantly assign to new direction (steer speed does need to be fast enough though)
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

        //display AI speed in console
        //print(currentSpeed);
        //print(localVel.z);
        //print("DotLR: " + rotDirLR + " DotFB: " + rotDirFB);

        //draw a debug line to see target position
        Debug.DrawLine(targetPos + new Vector3(0f, 50f, 0f), targetPos, Color.blue);
    }

    void FixedUpdate()
    {
        //create acceleration values
        float acceleration;

        //update acceleration values
        #region
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
        #endregion

        //Create and determine turn amount to alter acceleration
        #region
        float turnMultiplier = 1f;
        //New version
        //If target is behind car
        if (rotDirFB < 0)
        {
            turnMultiplier = 1f;
        }
        else
        {
            turnMultiplier = 1.1f - Mathf.Abs(rotDirLR);

            if(turnMultiplier > 1f)
            {
                turnMultiplier = 1f;
            }
        }
        //print(turnMultiplier);

        //---------- This is original version (keeping in case bugs happen and want to try with previous version)
        /*//If target is behind car
        if(rotDirFB < 0)
        {
            turnMultiplier = 1f;
        }
        else if (Mathf.Abs(rotDirLR) > 0.8f)
        {
            //else if target is far to the right/left of the AI car
            turnMultiplier = 0.2f;
        }
        else if (Mathf.Abs(rotDirLR) > 0.6f)
        {
            //else if target is medium/far to the right/left of the AI car
            turnMultiplier = 0.4f;
        }
        else if (Mathf.Abs(rotDirLR) > 0.4f)
        {
            //else if target is slightly/medium to the right/left of the AI car
            turnMultiplier = 0.6f;
        }
        else if (Mathf.Abs(rotDirLR) > 0.2f)
        {
            //else if target is slightly to the right/left of the AI car
            turnMultiplier = 0.8f;
        }
        else
        {
            //else if target is straight on or very slightly to the right/left of the AI car
            turnMultiplier = 1f;
        }*/
        //----------
        #endregion

        //apply acceleration
        #region
        //if not at top speed and not in radius of satisfaction of target position
        if (currentSpeed < topSpeed && Vector3.Distance(targetPos, transform.position) > radiusOfSat)
        {
            //(Vector3.Distance(targetPos, transform.position) [5 is to increase hpw fast the car accelerates]/ (currentSpeed + 1)) -
            //- makes the amount the car speeds up be based on how far the car is from the target devided by the current speed of the car
            brWheel.motorTorque = acceleration * turnMultiplier * ((Vector3.Distance(targetPos, transform.position) + 5) / (currentSpeed + 1));
            blWheel.motorTorque = acceleration * turnMultiplier * ((Vector3.Distance(targetPos, transform.position) + 5) / (currentSpeed + 1));
        }
        else
        {
            //else at top speed or in radius of satisfaction of target position
            brWheel.motorTorque = 0;
            blWheel.motorTorque = 0;
        }
        #endregion

        //apply brakes
        #region
        //if within the radius of satisfaction of the target position OR target is behind and local velocity is forward OR taget is in front and local velocity is backwards
        if (Vector3.Distance(targetPos, transform.position) < radiusOfSat || (rotDirFB < 0 && localVel.z > 0) || (rotDirFB > 0 && localVel.z < 0))
        {
            //car's natural deceleration
            rb.velocity *= 0.997f;
            //apply actual brakes to wheel colliders for added deceleration
            brWheel.brakeTorque = brakeForce;
            blWheel.brakeTorque = brakeForce;
            frWheel.brakeTorque = brakeForce;
            flWheel.brakeTorque = brakeForce;
        }
        else
        {
            //else not within radius of satisfaction
            brWheel.brakeTorque = 0;
            blWheel.brakeTorque = 0;
            frWheel.brakeTorque = 0;
            flWheel.brakeTorque = 0;
        }
        #endregion
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
