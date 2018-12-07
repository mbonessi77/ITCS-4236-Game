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
    [Range(0,4)]
    private int waypointChildInt;
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
    private List<Node> fullPath = new List<Node>();

    [SerializeField]
    private AudioSource engineAudio;
    [SerializeField]
    private AudioClip runningEngineClip, idleEngineClip;

    //a* script
    [SerializeField]
    private AStarSearch aStar;

    private int[] waypointIndex;

    void Start()
    {
        //Start with the target position being where the car is
        nextTarget = new Node(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), 0);
        targetPos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));

        Invoke("PathCreation", 3f);

        //----------For testing pathfinding and pathfollowing
        Invoke("CalculatePathTestFunction", 2f);
        //----------

        //set all engine audio source settings
        engineAudio.clip = idleEngineClip;
        engineAudio.pitch = 1f;
        engineAudio.volume = 1f;
        engineAudio.Play();
    }

    void PathCreation()
    {
        //create the waypointIndex array to hold to randomly selected waypoints along the track
        waypointIndex = new int[Waypoint_Cache.waypoints.Count];

        //print(Waypoint_Cache.waypoints.Count);
        //print(Waypoint_Cache.waypoints[0].transform.position.x);
        //print(Waypoint_Cache.waypoints[Waypoint_Cache.waypoints.Count-1].transform.position.x);

        //randomly assign the selected waypoints at each area
        for (int i = 0; i < Waypoint_Cache.waypoints.Count; i++)
        {
            waypointIndex[i] = waypointChildInt;//Random.Range(0, 5);
        }
        //print(waypointIndex.Length);

        //create and assign the starting point and goal point for the first waypoint
        Transform startWaypoint = transform;
        Transform goalWaypoint = Waypoint_Cache.waypoints[0].transform.GetChild(waypointIndex[0]);

        //special case for the first waypoint starting from the ai car's location
        bestPath = new Stack<Node>(new Stack<Node>(aStar.CalulatePath(startWaypoint.position.x, startWaypoint.position.z, goalWaypoint.position.x, goalWaypoint.position.z)));

        //pop off the first waypoint, to ensure no duplicates in the fullPath list
        bestPath.Pop();

        //pop off all node locations and add to the full path list to follow from start to finish
        while (bestPath.Count > 0)
        {
            fullPath.Add(bestPath.Pop());
        }

        //for all remaining track waypoint segments
        for (int i = 1; i < Waypoint_Cache.waypoints.Count; i++)
        {
            //assign starting waypoint location of segment and goal waypoint location of segment
            startWaypoint = Waypoint_Cache.waypoints[i - 1].transform.GetChild(waypointIndex[i - 1]);
            goalWaypoint = Waypoint_Cache.waypoints[i].transform.GetChild(waypointIndex[i]);

            //generate the path for the segment
            bestPath = new Stack<Node>(new Stack<Node>(aStar.CalulatePath(startWaypoint.position.x, startWaypoint.position.z, goalWaypoint.position.x, goalWaypoint.position.z)));

            //pop off the first waypoint, to ensure no duplicate waypoints in the fullPath list
            bestPath.Pop();

            //print(i + " Path");

            //pop off all node locations and add to the full path list to follow from start to finish
            while (bestPath.Count > 0)
            {
                fullPath.Add(bestPath.Pop());
            }
        }

        //GameManager.raceIsStarting = false;
    }

    void CalculatePathTestFunction()
    {
        //bestPath = new Stack<Node>(new Stack<Node>(aStar.CalulatePath(500, 25)));
    }

    void Update()
    {
        if (!GameManager.raceIsStarting)
        {
            //the ai never stop accelerating, so just set the clip to runningEngineClip
            if (engineAudio.clip != runningEngineClip)
            {
                engineAudio.clip = runningEngineClip;
            }

            if (engineAudio.isPlaying == false)
            {
                engineAudio.Play();
            }

            //adjust engine pitch with speed of car
            engineAudio.pitch = 1 + (currentSpeed / topSpeed) / 2;

            //if there is another node postion in the stack and (car is within the radius of satisfaction OR target waypoint is backwards on the track), pop the next node off and set as next target
            if (fullPath.Count > 0 && (Vector3.Distance(transform.position, targetPos) < radiusOfSat || targetPos.x - transform.position.x <= 0))
            {
                nextTarget = fullPath[0];
                fullPath.RemoveAt(0);
                targetPos = new Vector3(nextTarget.GetPosX(), transform.position.y, nextTarget.GetPosZ());
                //print(nextTarget.ToString());
            }/*
        else if (bestPath.Count <= 0)
        {
            Transform waypoint = Waypoint_Cache.waypoints[waypoint_int].transform.GetChild(Random.Range(0, 5));
            bestPath = new Stack<Node>(new Stack<Node>(aStar.CalulatePath(waypoint.position.x, waypoint.position.z)));
            waypoint_int += 1;
        }*/

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
            //print(rotDirLR);

            //draw a debug line to see target position
            Debug.DrawLine(targetPos + new Vector3(0f, 50f, 0f), targetPos, Color.blue);
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.raceIsStarting)
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

                if (turnMultiplier > 1f)
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
                brWheel.motorTorque = acceleration * turnMultiplier * ((Vector3.Distance(targetPos, transform.position) /*+ 5*/) / (currentSpeed + 1));
                blWheel.motorTorque = acceleration * turnMultiplier * ((Vector3.Distance(targetPos, transform.position) /*+ 5*/) / (currentSpeed + 1));
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
            //Original
            //Vector3.Distance(targetPos, transform.position) < radiusOfSat || (rotDirFB < 0 && localVel.z > 0) || (rotDirFB > 0 && localVel.z < 0) || (Mathf.Abs(rotDirLR) > 0.3f && localVel.z > (topSpeed * 0.66f))

            //if within the radius of satisfaction * .5 of the target position OR target is behind and local velocity is forward OR taget is in front and local velocity is backwards OR need to turn and going faster than certain speed
            if (Vector3.Distance(targetPos, transform.position) < (radiusOfSat * 0.5f) || (rotDirFB < 0 && localVel.z > 0) || (rotDirFB > 0 && localVel.z < 0) || (Mathf.Abs(rotDirLR) > 0.3f && localVel.z > (topSpeed * 0.7f)))
            {
                //car's natural deceleration
                //rb.velocity *= 0.997f;
                rb.velocity *= 0.998f;
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

            /* (Maybe add somewhere)
            RaycastHit hit;
            int layerMask1 = 1 << 9;
            int layerMask2 = 1 << 10;
            Debug.DrawRay(transform.position, transform.forward * 5f, Color.red);
            int layerMask3 = (layerMask1 | layerMask2);
            Physics.Raycast(transform.position + (-transform.right * 1.8f), -transform.right, out hit, 5f, layerMask3);
            */
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
