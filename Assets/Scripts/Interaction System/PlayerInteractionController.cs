using TMPro;
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
        Vector3 center = transform.position + transform.forward * 0.2f + Vector3.up * 0.2f;
        Vector3 extends = new Vector3(0.2f, 0.4f, 0.2f);
        Quaternion orientation = transform.rotation;
        Collider[] hits = Physics.OverlapBox(center, extends, transform.rotation, interactableLayer);
        DrawBox(center, extends, orientation, Color.cyan);

        if (hits.Length > 0)
        {
            Debug.Log("Hit: " + hits[0].name);
            DrawBox(center, extends, orientation, Color.darkGreen);
            currentTarget = hits[0].GetComponent<IInteractable>();
            if (currentTarget != null)
            {
                // hier ui einblenden oder so
                return;
            }
        }
        currentTarget = null;
        // ui wieder hiden
    }
    void DrawBox(Vector3 center, Vector3 extents, Quaternion orientation, Color color)
    {
        Vector3[] points = new Vector3[8];

        // Alle 8 Ecken der Box generieren
        points[0] = center + orientation * new Vector3(extents.x, extents.y, extents.z);
        points[1] = center + orientation * new Vector3(-extents.x, extents.y, extents.z);
        points[2] = center + orientation * new Vector3(-extents.x, -extents.y, extents.z);
        points[3] = center + orientation * new Vector3(extents.x, -extents.y, extents.z);

        points[4] = center + orientation * new Vector3(extents.x, extents.y, -extents.z);
        points[5] = center + orientation * new Vector3(-extents.x, extents.y, -extents.z);
        points[6] = center + orientation * new Vector3(-extents.x, -extents.y, -extents.z);
        points[7] = center + orientation * new Vector3(extents.x, -extents.y, -extents.z);

        // Box-Kanten zeichnen
        for (int i = 0; i < 4; i++)
        {
            Debug.DrawLine(points[i], points[(i + 1) % 4], color, 0.1f);
            Debug.DrawLine(points[i + 4], points[((i + 1) % 4) + 4], color, 0.1f);
            Debug.DrawLine(points[i], points[i + 4], color, 0.1f);
        }
    }
}
