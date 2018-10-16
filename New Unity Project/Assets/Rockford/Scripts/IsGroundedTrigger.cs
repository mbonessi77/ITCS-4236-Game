using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGroundedTrigger : MonoBehaviour {

    [SerializeField]
    private CartoonMovement movementScript;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Ground")
        {
            movementScript.isGrounded = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Ground")
        {
            movementScript.isGrounded = false;
        }
    }
}
