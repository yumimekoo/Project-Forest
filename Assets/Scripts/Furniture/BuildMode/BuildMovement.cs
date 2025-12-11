using UnityEngine;
using UnityEngine.InputSystem;

public class BuildMovement : MonoBehaviour
{
    public InputActionAsset InputActions;
    public InputAction moveAction;
    public GameObject objectToMove;

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
    void Update()
    {
        if(GameState.isInBuildMode && objectToMove != null)
        {
            Vector2 movement = moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(movement.x, 0, movement.y);
            objectToMove.transform.position += move * Time.deltaTime * 5f; // Geschwindigkeit anpassen
        }
    }
}
