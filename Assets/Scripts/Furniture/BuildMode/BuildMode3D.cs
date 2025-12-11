using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class BuildMode3D : MonoBehaviour
{
    public DynamicGrid grid;
    public Camera mainCamera;

    private FurnitureSO currentItem;
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
        if (!isPlacing)
            return;

        HandleDeleteToggle();
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
            placingMode = !deleteMode;

            if (deleteMode && preview != null)
            {
                preview.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

    // ---
    // Main Logic
    // ---

    private void HandlePlacementOrDeletion()
    {
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
        if(deleteMode)
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
        Instantiate(currentItem.furniturePrefab, position, Quaternion.identity);
        occupiedCells.Add(cell);
        FurniturePlacementManager.Instance.RegisterPlacement(currentItem.numericID, cell, 0);
    }

    private void TryDelete(Vector2Int cell)
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Furniture"))
        {
            if (grid.WorldToGrid(obj.transform.position) == cell)
            {
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


    //public void StartPlacement()
    //{
    //    Ray ray = mainCamera.ScreenPointToRay(pointerPos.ReadValue<Vector2>());
    //    if (!Physics.Raycast(ray, out RaycastHit hitInfo))
    //    {
    //        return;
    //    }

    //    Vector2Int cellPosition = grid.WorldToGrid(hitInfo.point);
    //    if (grid.IsInsideGrid(cellPosition.x, cellPosition.y))
    //    {
    //        Vector3 snapPos = grid.GetWorldPosition(cellPosition.x, cellPosition.y);
    //        preview.transform.position = snapPos;
    //    }

    //    preview.GetComponent<Renderer>().material.color =
    //        occupiedCells.Contains(cellPosition) ? Color.red : Color.green;

    //    if (clickAction.WasPressedThisFrame() && !occupiedCells.Contains(cellPosition))
    //    {
    //        Vector2Int p = grid.WorldToGrid(hitInfo.point);
    //        if (!grid.IsInsideGrid(p.x, p.y))
    //        {
    //            Debug.Log("Out of bounds");
    //            return;
    //        }
    //        Vector3 spawn = grid.GetWorldPosition(p.x, p.y);
    //        Instantiate(currentItem.furniturePrefab, spawn, Quaternion.identity);
    //        occupiedCells.Add(cellPosition);
    //        FurniturePlacementManager.Instance.RegisterPlacement(currentItem.numericID, p, 0);
    //        return;
    //    }
    //    if (clickAction.WasPressedThisFrame() && occupiedCells.Contains(cellPosition))
    //    {
    //        RemoveAt(cellPosition);
    //        occupiedCells.Remove(cellPosition);
    //        FurniturePlacementManager.Instance.RemovePlacement(cellPosition);
    //        Debug.Log("Cell is already occupied.");
    //    }
    //}

    //private void RemoveAt(Vector2Int cellPosition)
    //{
    //    foreach (var obj in GameObject.FindGameObjectsWithTag("Furniture"))
    //    {
    //        Vector2Int objCell = grid.WorldToGrid(obj.transform.position);
    //        if (objCell == cellPosition)
    //        {
    //            Destroy(obj);
    //        }
    //    }
    //}

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
            Instantiate(so.furniturePrefab, spawn, rotation);
            occupiedCells.Add(new Vector2Int(item.x, item.y));

        }
    }
}
