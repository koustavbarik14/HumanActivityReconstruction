using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Public variable to assign the cube in the Inspector
    public Transform cubeTransform;

    // Offset to maintain the camera's position relative to the cube
    public Vector3 offset;

    [SerializeField] private float smoothTime;
    private Vector3 _currentVelocity = Vector3.zero;

    private void Awake()
    {
        offset = transform.position - cubeTransform.position;

    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (cubeTransform != null)
        {
            Vector3 targetPosition = cubeTransform.position + offset;
            // Update the camera's position to follow the cube, maintaining the offset
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);
            // Optional: make the camera look at the cube
            //transform.LookAt(cubeTransform);
        }
    }
}
