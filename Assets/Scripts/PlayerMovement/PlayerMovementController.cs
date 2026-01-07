using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    public float speed = 5f;
    public Camera cam; 
    public Rigidbody rb;


    public InputActionAsset InputActions;
    public InputAction moveAction;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }
    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }
    void FixedUpdate()
    {
        if (!GameState.playerMovementAllowed || GameState.isInBuildMode)
            return;
        Vector3 movement = moveAction.ReadValue<Vector2>();
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camForward * movement.y + camRight * movement.x) * speed;
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 0.4f);
        }
    }

}
