using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float trueTopSpeed;
    [SerializeField]
    private Rigidbody rb;
    private Vector3 localVel;
    private Vector3 inputVector;
    [SerializeField]
    private AudioSource runningEngineAudio, idleEngineAudio;
    [SerializeField]
    private AudioClip runningEngineClip, idleEngineClip;

    void Start()
    {
        GameManager.raceIsStarting = true;
        GameManager.raceFinished = false;
        trueTopSpeed = topSpeed;

        //set all engine audio source settings
        runningEngineAudio.clip = runningEngineClip;
        runningEngineAudio.pitch = 1f;
        runningEngineAudio.volume = 0f;
        idleEngineAudio.clip = idleEngineClip;
        idleEngineAudio.pitch = 1.25f;
        idleEngineAudio.volume = 0.5f;
        idleEngineAudio.Play();
    }

    void Update()
    {
        //if you want to skip to the end screen (press T)
        if (Input.GetKeyDown(KeyCode.T))
        {
            //set the race to finished (which causes the GUIManager script to store time values and then load the end screen)
            GameManager.raceFinished = true;
            //SceneManager.LoadScene("EndScreen");
        }

        //if escape is pressed
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //return to main menu
            SceneManager.LoadScene("MainMenu(RockWork)");
        }

        if (!GameManager.raceIsStarting)
        {
            //offroad code
            #region
            //get layer mask for track objects
            int trackLayerMask = 1 << 8;

            //if raycast from the center of the player's car straight down hits a track, then the player is on the track and can move at full speed
            if(Physics.Raycast(transform.position, Vector3.down, 5f, trackLayerMask))
            {
                Debug.DrawRay (transform.position, Vector3.down * 5f, Color.magenta);
                //reset top speed to the true top speed
                topSpeed = trueTopSpeed;
            }
            else
            {
                //else the player is offroad and can only move at half speed
                topSpeed = trueTopSpeed * 0.5f;
            }
            #endregion

            //engine sound code
            #region
            //adjust the engine sound pitch with speed
            runningEngineAudio.pitch = 1 + (currentSpeed / trueTopSpeed) / 2;

            //if the player is accelerating
            if (Mathf.Abs(inputVector.z) > 0.1f)
            {
                //adjust audio sources to trasition from idle to running
                if(runningEngineAudio.volume < 0.5f)
                {
                    runningEngineAudio.volume += 0.2f * Time.deltaTime;
                }

                if (idleEngineAudio.volume > 0f)
                {
                    idleEngineAudio.volume -= 0.4f * Time.deltaTime;
                }
            }
            else
            {
                //adjust audio sources to trasition from running to idle
                if (idleEngineAudio.volume < 0.5f)
                {
                    idleEngineAudio.volume += 0.1f * Time.deltaTime;
                }

                if (runningEngineAudio.volume > 0f)
                {
                    runningEngineAudio.volume -= 0.15f * Time.deltaTime;
                }
            }
            #endregion

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
    }

    void FixedUpdate()
    {
        if (!GameManager.raceIsStarting)
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
            else if (inputVector.z == 0 || currentSpeed > (topSpeed + 2))
            {
                //if not pressing acceleration OR traveling faster than the top speed
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

    private void OnTriggerEnter(Collider other)
    {
        //when player hits the finish line
        if (other.tag == "FinishLine")
        {
            //set the race to finished (which causes the GUIManager script to store time values and then load the end screen)
            GameManager.raceFinished = true;
            //SceneManager.LoadScene("EndScreen");
        }
    }
}
