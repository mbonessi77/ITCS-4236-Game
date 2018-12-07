using UnityEngine;
using System.Collections;
public class Car : MonoBehaviour
{
public Transform centerOfMass;
public WheelCollider[] wheelColliders = new WheelCollider[4];
public Transform[] tireMeshes = new Transform[4];
private Rigidbody m_rigidBody;
public float topSpeed = 100; // km per hour
private float currentSpeed = 0;
private float pitch = 0;
private float maxTorque;
void Start()
{
m_rigidBody = GetComponent<Rigidbody>();
m_rigidBody.centerOfMass = centerOfMass.localPosition;
}
void Update()
{
UpdateMeshesPositions();
currentSpeed = transform.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
pitch = currentSpeed / topSpeed;
GetComponent<AudioSource>().pitch = pitch;
}
void FixedUpdate()
{
float steer = Input.GetAxis("Horizontal");
float accelerate = Input.GetAxis("Vertical");
float finalAngle = steer * 45f;
wheelColliders[0].steerAngle = finalAngle;
wheelColliders[1].steerAngle = finalAngle;
for(int i = 0; i < 4; i++)
{
wheelColliders[i].motorTorque = accelerate * maxTorque;
}
}
void UpdateMeshesPositions()
{
for(int i = 0; i < 4; i++)
{
Quaternion quat;
Vector3 pos;
wheelColliders[i].GetWorldPose(out pos, out quat);
tireMeshes[i].position = pos;
tireMeshes[i].rotation = quat;
}
}
}
