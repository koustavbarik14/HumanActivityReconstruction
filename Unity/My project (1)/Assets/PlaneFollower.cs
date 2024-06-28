using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneFollower : MonoBehaviour
{
    // Public variable to assign the cube in the Inspector
    public Transform cubeTransform;

    // Offset to maintain the plane's position relative to the cube
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if (cubeTransform != null)
        {
            // Calculate the initial offset
            offset = transform.position - cubeTransform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cubeTransform != null)
        {
            // Update the plane's position to follow the cube, maintaining the offset
            transform.position = cubeTransform.position + offset;
        }
    }
}
