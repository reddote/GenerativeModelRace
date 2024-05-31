using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target; // The target the camera will follow (your car)
    public float smoothSpeed = 0.125f; // The speed of smoothing
    public Vector3 offset; // The offset between the camera and the target

    private void FixedUpdate()
    {
        // Calculate the desired position based on the target's position and rotation
        Vector3 desiredPosition = target.TransformPoint(offset);
        
        // Smoothly interpolate the position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Smoothly interpolate the rotation
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed);
        transform.rotation = smoothedRotation;
    }
}
