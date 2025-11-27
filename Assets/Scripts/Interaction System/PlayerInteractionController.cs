using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    public float interactionRange = 5f;
    public LayerMask interactableLayer;

    private IInteractable currentTarget;

    public InputActionAsset InputActions;
    public InputAction interactAction;
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
        interactAction = InputSystem.actions.FindAction("Interact");
    }
    private void Update()
    {
        CheckForInteractable();
        if (interactAction.WasPressedThisFrame())
        { 
            Debug.Log("Interact action pressed"); 
        }
        if (currentTarget != null && interactAction.WasPressedThisFrame())
        {
            Debug.Log("Interacting with " + currentTarget);
            currentTarget.Interact(PlayerInventory.Instance);
        }
    }

    private void CheckForInteractable()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        Vector3 dir = transform.forward;
        Debug.DrawRay(origin, dir * interactionRange, Color.blue);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, interactionRange, interactableLayer))
        {
            Debug.Log("Hit: " + hit.collider.name);
            Debug.DrawLine(origin, hit.point, Color.green);
            currentTarget = hit.collider.GetComponent<IInteractable>();
            if (currentTarget != null)
            {
                // hier ui einblenden oder so
                return;
            }
        }
        currentTarget = null;
        // ui wieder hiden
    }
}
