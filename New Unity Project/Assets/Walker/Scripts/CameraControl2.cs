using UnityEngine;

public class CameraControl2 : MonoBehaviour
{
    [SerializeField]
    private Transform cameraControl;
    [SerializeField]
    private Transform carCam;
    [SerializeField]
    private Transform car;
    [SerializeField]
    private Rigidbody carPhysics;

    [SerializeField]
    [Tooltip("If car speed is below this value, then the camera will default to looking forwards.")]
    private float rotationThreshold = 1f;
    
    [SerializeField]
    [Tooltip("How closely the camera follows the car's position. The lower the value, the more the camera will lag behind.")]
    private float cameraStickiness = 10.0f;
    
    [SerializeField]
    [Tooltip("How closely the camera matches the car's velocity vector. The lower the value, the smoother the camera rotations, but too much results in not being able to see where you're going.")]
    private float cameraRotationSpeed = 5.0f;

    void FixedUpdate()
    {
        Quaternion look;

        // Moves the camera to match the car's position.
        cameraControl.position = Vector3.Lerp(cameraControl.position, car.position, cameraStickiness * Time.fixedDeltaTime);

        // If the car isn't moving, default to looking forwards. Prevents camera from freaking out with a zero velocity getting put into a Quaternion.LookRotation
        if (carPhysics.velocity.magnitude < rotationThreshold)
            look = Quaternion.LookRotation(car.forward);
        else
            look = Quaternion.LookRotation(carPhysics.velocity.normalized);
        
        // Rotate the camera towards the velocity vector.
        look = Quaternion.Slerp(cameraControl.rotation, look, cameraRotationSpeed * Time.fixedDeltaTime);                
        cameraControl.rotation = look;
    }
}