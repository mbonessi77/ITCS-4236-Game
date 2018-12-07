using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartoonMovement : MonoBehaviour
{
    /// <summary>
    /// This needs a bit of work!!!!!!!!
    /// </summary>

    [SerializeField]
    private float topSpeed, topAcceleration;
    private float currentSpeed, steer;
    [HideInInspector]
    public float acceleration;
    private Vector3 localVelocity;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform trans;
    [HideInInspector]
    public bool isGrounded;

    void Start()
    {
        
    }

    void Update()
    {
        //store speed
        currentSpeed = rb.velocity.magnitude;

        //determine correct acceleration value
        //if accelerating forwards and on ground
        if (Input.GetAxis("Vertical") > 0.2f && isGrounded)
        {
            //if going forwards and not at top speed, speed up forwards
            if (acceleration < topAcceleration)
            {
                acceleration += 0.2f;
            }
        }
        else if (Input.GetAxis("Vertical") < -0.2f && isGrounded)
        {
            //else if accelerating backwards and on ground

            //if going backwards and not at half of top speed in reverse, speed up backwards
            if (acceleration > -(topAcceleration * 0.5f))
            {
                acceleration -= 0.2f;
            }
        }
        else
        {
            //else not accelerating and/or not on ground

            //if not accelerating forwards or backwards and speed is above zero, slow down forward movement
            if (acceleration > 0.1f)
            {
                acceleration -= 0.1f;
            }
            else if (acceleration < -0.1f)
            {
                //else if not accelerating forwards or backwards and speed is below zero, slow down reverse movement
                acceleration += 0.1f;
            }
            else
            {
                //else set speed to zero
                acceleration = 0;
            }
        }

        //user applied brakes
        if (Input.GetKey(KeyCode.Space))
        {
            if(acceleration > 0.4f)
            {
                acceleration -= 0.4f;
            }
            else if(acceleration < -0.4f)
            {
                acceleration += 0.4f;
            }
        }

        //set turn ability based on speed
        if(currentSpeed > (topSpeed * 0.66f))
        {
            steer = 0.75f;
        }
        else if(currentSpeed > (topSpeed * 0.33f))
        {
            steer = 1f;
        }
        else
        {
            steer = 1.25f;
        }

        //create variable to track speed and direction in the .z portion of the Vector3 [UNUSED!!!]
        localVelocity = rb.transform.InverseTransformDirection(rb.velocity);

        //print(currentSpeed);
    }

    void FixedUpdate()
    {
        //create movement direction
        Vector3 towards = trans.forward;

        //if not currently going max speed
        //update velocity values/accelerate
        if (currentSpeed < topSpeed && isGrounded)
        {
            //normalize vector to get just the direction
            towards.Normalize();
            towards *= acceleration;

            //move car
            rb.velocity = towards;
        }
        else if(!isGrounded)
        {
            rb.velocity *= 0.9f;
        }

        //create target rotation variable
        Quaternion targetRotation = Quaternion.LookRotation(trans.forward);

        //if steering right and moving
        if (Input.GetAxis("Horizontal") > 0.1f && Mathf.Abs(currentSpeed) > 1f)
        {
            targetRotation = Quaternion.LookRotation(trans.forward + (trans.forward + trans.right));
        }
        else if (Input.GetAxis("Horizontal") < -0.1f && Mathf.Abs(currentSpeed) > 1f)
        {
            //else if steering left and moving
            targetRotation = Quaternion.LookRotation(trans.forward + (trans.forward - trans.right));
        }

        //rotate car to desired steering position
        trans.rotation = Quaternion.Lerp(trans.rotation, targetRotation, steer * Time.fixedDeltaTime);
    }
}
