using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera followCamera;
    private Transform currentTargetFollow;
    public Transform playerFollowObject;
    public Transform buildFollowObject;

    public Vector3 offset;
    public float smoothSpeed;

    public void Awake()
    {
        currentTargetFollow = playerFollowObject;
    }
    void LateUpdate()
    {
        if (currentTargetFollow == null || followCamera == null)
            return;
        Vector3 desPos = currentTargetFollow.position + offset;
        Vector3 smoothPos = Vector3.Lerp(followCamera.transform.position, desPos, smoothSpeed);
        followCamera.transform.position = smoothPos;
    }

    public void ChangeFollowTarget(bool buildModeActive)
    {
        if (buildModeActive)
        {
            currentTargetFollow = buildFollowObject;
            smoothSpeed = 0.05f;
            var e = followCamera.transform.eulerAngles;
            followCamera.transform.rotation = Quaternion.Euler(70, 0f, e.z);
            offset = new Vector3(0, 14, 0);
        }
        else if (!buildModeActive)
        {
            currentTargetFollow = playerFollowObject;
            smoothSpeed = 0.03f;
            
            var e = followCamera.transform.eulerAngles;
            followCamera.transform.rotation = Quaternion.Euler(50, -45f, e.z);
            offset = new Vector3(3, 7, -3);
        }
    }
}

