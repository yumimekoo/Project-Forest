using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    public float speed = 5f;
    public float speedMultiplier = 1.6f;
    public Camera cam; 
    public Rigidbody rb;


    public InputActionAsset InputActions;
    public InputAction moveAction;
    public InputAction sprintAction;

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
        sprintAction = InputSystem.actions.FindAction("Sprint");
    }
    void FixedUpdate()
    {
        if (!GameState.playerMovementAllowed || GameState.isInBuildMode)
            return;
        
        Vector3 movement = moveAction.ReadValue<Vector2>();

        bool isSprinting = sprintAction.IsPressed();
        float currentSpeed = isSprinting ? speed * speedMultiplier : speed;
        
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camForward * movement.y + camRight * movement.x) * currentSpeed;
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 0.4f);
        }
    }

}
