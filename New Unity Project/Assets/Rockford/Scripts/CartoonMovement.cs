using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartoonMovement : MonoBehaviour
{
    [SerializeField]
    private float topSpeed, steer, brake;
    private float currentSpeed, acceleration;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform trans;

    void Start()
    {
        
    }

    void Update()
    {
        //store speed
        currentSpeed = rb.velocity.magnitude;

        //create acceleration values
        Vector3 towards = trans.forward;
        Quaternion targetRotation = Quaternion.LookRotation(trans.forward);

        //if steering right
        if (Input.GetAxis("Horizontal") > 0.1f)
        {
            targetRotation = Quaternion.LookRotation(trans.forward + (trans.forward + trans.right));
        }
        else if(Input.GetAxis("Horizontal") < -0.1f)
        {
            //else if steering right
            targetRotation = Quaternion.LookRotation(trans.forward + (trans.forward - trans.right));
        }

        //determine correct acceleration value
        //if accelerating forwards
        if (Input.GetAxis("Vertical") > 0.1f)
        {
            //if going forwards and not at top speed, speed up backwards
            if (acceleration < topSpeed)
            {
                acceleration += 0.1f;
            }
        }
        else if (Input.GetAxis("Vertical") < -0.1f)
        {
            //else if accelerating backwards

            //if going backwards and not at half of top speed in reverse, speed up backwards
            if (acceleration > -(topSpeed * 0.5f))
            {
                acceleration -= 0.1f;
            }
        }
        else
        {
            //else not accelerating

            //if not accelerating forwards or backwards and speed is above zero, slow down forward movement
            if (acceleration > 0.05f)
            {
                acceleration -= 0.05f;
            }
            else if (acceleration < -0.05f)
            {
                //else if not accelerating forwards or backwards and speed is below zero, slow down reverse movement
                acceleration += 0.05f;
            }
            else
            {
                //else set speed to zero
                acceleration = 0;
            }
        }

        //if not currently going max speed
        //update acceleration values/accelerate
        if (currentSpeed < topSpeed)
        {
            //normalize vector to get just the direction
            towards.Normalize();
            towards *= acceleration;

            //move the leader
            rb.velocity = towards;
        }

        //user applied brakes
        if (Input.GetKey(KeyCode.Space))
        {
            if(acceleration > 0.2f)
            {
                acceleration -= 0.2f;
            }
            else if(acceleration < -0.2f)
            {
                acceleration += 0.2f;
            }
        }

        //rotate car to desired steering position
        trans.rotation = Quaternion.Lerp(trans.rotation, targetRotation, steer * Time.deltaTime);

        print(rb.velocity.magnitude);
    }
}
