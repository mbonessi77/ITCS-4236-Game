using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
AudioSource audioSource;

void Start()
{
 audioSource = GetComponent<AudioSource>();
}

void OnCollisionEnter(Collision collision)
{
foreach (ContactPoint contact in collision.contacts)
{
Debug.DrawRay(contact.point, contact.normal, Color.white);
}
if (collision.relativeVelocity.magnitude > 2)
 audioSource.Play();
}
}