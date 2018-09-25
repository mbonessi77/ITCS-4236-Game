using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emptyObjectFollow2 : MonoBehaviour
{
    public Transform target;
    private float damping = 10000.0f;

    void Update()
    {
        if (!target)
        {
            GameObject go = GameObject.FindWithTag("Player");
            if (go)
            {
                target = go.transform;
            }
        }

        if (target)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * damping);

            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * 5.0f);
        }

    }
}
