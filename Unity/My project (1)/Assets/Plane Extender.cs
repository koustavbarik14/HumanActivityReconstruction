using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneExtender : MonoBehaviour
{
    public Transform cubeTransform;
    public float extensionRate = 5.0f;

    void Update()
    {
        // Extend the plane based on the cube's z position
        if (cubeTransform.position.z > transform.position.z)
        {
            Vector3 newScale = transform.localScale;
            newScale.z = cubeTransform.position.z;
            transform.localScale = newScale;

            // Optional: Adjust the plane's position to keep it centered
            Vector3 newPosition = transform.position;
            newPosition.z = newScale.z / 2.0f;
            transform.position = newPosition;
        }
    }
}
