using System;
using UnityEngine;

public class DemoCameraMovement : MonoBehaviour
{
    public float lerpSpeed;
    public float deltaFactor;

    public float yAngle;
    public float xAngle;
    
    public Vector3 targetOffset;
    public Vector3 cameraOffset;

    public Vector3 targetPosition;

    public GameObject target;

    public void UpdateAngle(float yd, float xd)
    {
        yAngle += yd * deltaFactor;
        xAngle -= xd * deltaFactor;
    }
    
    private void FixedUpdate()
    {
        targetPosition = Vector3.Lerp(targetPosition, target.transform.position, lerpSpeed * Time.deltaTime);
        
        xAngle = Mathf.Clamp(xAngle, -90, 90);

        Quaternion rotation = Quaternion.Euler(xAngle, yAngle , 0);
        
        Vector3 desiredPosition = targetPosition + rotation * cameraOffset;
        
        transform.position = desiredPosition;
        transform.LookAt(targetPosition + targetOffset); 
    }
}
