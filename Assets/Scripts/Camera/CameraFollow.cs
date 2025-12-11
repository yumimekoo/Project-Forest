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
    // Update is called once per frame
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
            return;
        }
        else if (!buildModeActive)
        {
            currentTargetFollow = playerFollowObject;
            smoothSpeed = 0.03f;
            return;
        }
        Debug.Log("Error changing camera follow target");
    }
}

