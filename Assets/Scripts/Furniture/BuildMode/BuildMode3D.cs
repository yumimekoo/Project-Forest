using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BuildMode3D : MonoBehaviour
{
    public DynamicGrid grid;
    public Camera mainCamera;
    public GameObject deletionPreviewPrefab;

    private FurnitureSO currentItem;
    private int rotY = 0;
    private GameObject preview;

    private bool isPlacing = false;
    private bool placingMode = false;
    private bool deleteMode = false;
    public bool isPointerOverUI = false;

    private HashSet<Vector2Int> occupiedCells = new();

    public InputActionAsset InputActions;
    public InputAction rotateAction;
    public InputAction clickAction;
    public InputAction rightClickAction;
    public InputAction pointerPos;
    private void Awake()
    {
        clickAction = InputSystem.actions.FindAction("LeftClick");
        rightClickAction = InputSystem.actions.FindAction("RightClick");
        rotateAction = InputSystem.actions.FindAction("Rotate");
        pointerPos = InputSystem.actions.FindAction("PointerPos");
    }


    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }
    // ---
    // UI Button Calls
    // ---

    public void StartBuild(FurnitureSO item)
    {
        deleteMode = false;
        placingMode = true;

        if (preview != null)
        {
            Destroy(preview);
        }

        currentItem = item;
        isPlacing = true;
        preview = Instantiate(item.furniturePreview);
    }

    public void StopBuild()
    {
        placingMode = false;
        deleteMode = false;
        isPlacing = false;

        if (preview != null)
        {
            Destroy(preview);
        }
    }

    private void Update()
    {
        if(!GameState.isInBuildMode)
            return;

        HandleDeleteToggle();

        if (!isPlacing)
            return;

        //HandleStopPlacementByButton();

        if(isPointerOverUI)
            return;

        HandlePlacementOrDeletion();
    }

    private void HandleDeleteToggle()
    {
        if (rightClickAction.WasPressedThisFrame())
        {
            deleteMode = !deleteMode;

            if (!isPlacing)
            {
                Debug.Log("Entering delete mode");
                isPlacing = true;
                deleteMode = true;
                preview = Instantiate(deletionPreviewPrefab);
                return;
            }
            if (deleteMode && preview != null)
            {
                Destroy(preview);
                Debug.Log("deleteMode && preview != null");
                preview = Instantiate(deletionPreviewPrefab);
                return;
            }
            if (!deleteMode && preview != null)
            {
                Debug.Log("!deleteMode && preview != null");
                Destroy(preview);
                isPlacing = false;
                return;
            }
        }
    }

    // ---
    // Main Logic
    // ---

    private void HandlePlacementOrDeletion()
    {
        if(rotateAction.WasPressedThisFrame() && !deleteMode)
        {
            rotY += 90;
            if(rotY > 270)
                rotY = 0;
        }
        Ray ray = mainCamera.ScreenPointToRay(pointerPos.ReadValue<Vector2>());
        if(!Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return;
        }

        Vector2Int cell = grid.WorldToGrid(hitInfo.point);
        bool inside = grid.IsInsideGrid(cell.x, cell.y);
        bool occupied = occupiedCells.Contains(cell);

        if (!inside)
            return;

        Vector3 snapPos = grid.GetWorldPosition(cell.x, cell.y);

        // DELETE MODE

        if (deleteMode)
        {
            Preview(cell, snapPos, occupied);
            if (clickAction.WasPressedThisFrame())
            {
                TryDelete(cell);
            }
            return;
        }

        // PLACING MODE

        if (placingMode)
        {
            Preview(cell, snapPos, occupied);

            if(clickAction.WasPressedThisFrame() && !occupied)
            {
                PlaceFurniture(cell, snapPos);
            }
        }
    }

    // ---
    // Preview 
    // ---

    private void Preview(Vector2Int cell, Vector3 snapPos, bool occupied)
    {
        if (preview == null)
            return;

        preview.transform.position = snapPos;
        preview.transform.rotation = Quaternion.Euler(0, rotY, 0);
        if (deleteMode)
        {
            preview.GetComponent<Renderer>().material.color = Color.red;
            return;
        }
        preview.GetComponent<Renderer>().material.color = occupied ? Color.red : Color.green;
    }

    // ---
    // Placement / Removal
    // ---

    private void PlaceFurniture(Vector2Int cell, Vector3 position)
    {
        if(!FurnitureInventory.Instance.Remove(currentItem.numericID))
        {
            Debug.LogWarning("Not enough items in inventory to place furniture.");
            return;
        }
        var go = Instantiate(currentItem.furniturePrefab, position, Quaternion.Euler(0, rotY, 0));
        go.AddComponent<FurnitureIdentifier>().so = currentItem;
        occupiedCells.Add(cell);
        FurniturePlacementManager.Instance.RegisterPlacement(currentItem.numericID, cell, rotY);
    }

    private void TryDelete(Vector2Int cell)
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Furniture"))
        {
            if (grid.WorldToGrid(obj.transform.position) == cell)
            {
                FurnitureSO soItem = obj.GetComponent<FurnitureIdentifier>().so;
                FurnitureInventory.Instance.Add(soItem.numericID);

                Destroy(obj);
                occupiedCells.Remove(cell);
                FurniturePlacementManager.Instance.RemovePlacement(cell);
                return;
            }
        }
    }

    // ---
    // Reload From Save
    // ---

    public IEnumerator RebuildFromSave(List<PlacedFurnitureData> items)
    {
        yield return null;
        foreach (var item in items)
        {
            FurnitureSO so = FurnitureDatabase.Instance.GetByID(item.id);
            if (so == null)
            {
                Debug.LogWarning($"FurnitureSO with ID {item.id} not found in database.");
                continue;
            }
            Vector3 spawn = grid.GetWorldPosition(item.x, item.y);
            Quaternion rotation = Quaternion.Euler(0, item.rotY, 0);
            var go = Instantiate(so.furniturePrefab, spawn, rotation);
            go.AddComponent<FurnitureIdentifier>().so = so;
            occupiedCells.Add(new Vector2Int(item.x, item.y));

        }
    }
}
