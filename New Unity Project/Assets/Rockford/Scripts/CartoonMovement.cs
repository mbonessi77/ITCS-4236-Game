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
        //create/update steering values
        float steerRotation = Input.GetAxis("Horizontal") * steer;

        //store speed
        currentSpeed = rb.velocity.magnitude;

        //create acceleration values
        Vector3 towards = Vector3.zero;

        //determine correct acceleration value
        //if accelerating forwards
        if (Input.GetAxis("Vertical") > 0.1f)
        {
            towards = trans.forward;

            //if going forwards and not at top speed, speed up backwards
            if (acceleration < topSpeed)
            {
                acceleration += 0.1f;
            }
        }
        else if (Input.GetAxis("Vertical") < -0.1f)
        {
            //else if accelerating backwards

            towards = trans.forward;

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

        //update acceleration values
        if (currentSpeed < topSpeed && Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
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

        }
        print(towards);
    }
}
