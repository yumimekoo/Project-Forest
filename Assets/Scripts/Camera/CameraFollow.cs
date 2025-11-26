using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera followCamera;
    public Transform targetFollow;

    public Vector3 offset;
    public float smoothSpeed;

    // Update is called once per frame
    void LateUpdate()
    {
        if (targetFollow == null || followCamera == null)
            return;
        Vector3 desPos = targetFollow.position + offset;
        Vector3 smoothPos = Vector3.Lerp(followCamera.transform.position, desPos, smoothSpeed);
        followCamera.transform.position = smoothPos;
    }
}