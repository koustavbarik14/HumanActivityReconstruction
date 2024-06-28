using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Public variable to assign the cube in the Inspector
    public Transform cubeTransform;

    // Offset to maintain the camera's position relative to the cube
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        if (cubeTransform != null)
        {
            // Update the camera's position to follow the cube, maintaining the offset
            transform.position = cubeTransform.position + offset;
            // Optional: make the camera look at the cube
            transform.LookAt(cubeTransform);
        }
    }
}
