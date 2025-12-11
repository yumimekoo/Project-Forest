using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildMode3D : MonoBehaviour
{
    public DynamicGrid grid;
    public Camera mainCamera;

    private FurnitureSO currentItem;
    private GameObject preview;

    private bool isPlacing = false;
    private bool placingMode = false;
    private bool deleteMode = false;

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

    public void StartBuild(FurnitureSO item)
    {

        if(preview != null)
        {
            Destroy(preview);
        }
        currentItem = item;
        isPlacing = true;
        preview = Instantiate(item.furniturePreview);
    }

    public void StopBuild()
    {
        
        if (preview != null)
        {
            Destroy(preview);
        }
        currentItem = null;
        isPlacing = false;
    }
    private void Update()
    {
        if (GameState.isInBuildMode && isPlacing)
        {
            StartPlacement();
        }
    }

    public void StartPlacement()
    {
        Ray ray = mainCamera.ScreenPointToRay(pointerPos.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return;
        }

        Vector2Int cellPosition = grid.WorldToGrid(hitInfo.point);
        if (grid.IsInsideGrid(cellPosition.x, cellPosition.y))
        {
            Vector3 snapPos = grid.GetWorldPosition(cellPosition.x, cellPosition.y);
            preview.transform.position = snapPos;
        }

        preview.GetComponent<Renderer>().material.color =
            occupiedCells.Contains(cellPosition) ? Color.red : Color.green;

        if (clickAction.WasPressedThisFrame() && !occupiedCells.Contains(cellPosition))
        {
            Vector2Int p = grid.WorldToGrid(hitInfo.point);
            if (!grid.IsInsideGrid(p.x, p.y))
            {
                Debug.Log("Out of bounds");
                return;
            }
            Vector3 spawn = grid.GetWorldPosition(p.x, p.y);
            Instantiate(currentItem.furniturePrefab, spawn, Quaternion.identity);
            occupiedCells.Add(cellPosition);
            FurniturePlacementManager.Instance.RegisterPlacement(currentItem.numericID, p, 0);
            return;
        }
        if (clickAction.WasPressedThisFrame() && occupiedCells.Contains(cellPosition))
        {
            RemoveAt(cellPosition);
            occupiedCells.Remove(cellPosition);
            FurniturePlacementManager.Instance.RemovePlacement(cellPosition);
            Debug.Log("Cell is already occupied.");
        }
    }

    private void RemoveAt(Vector2Int cellPosition)
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Furniture"))
        {
            Vector2Int objCell = grid.WorldToGrid(obj.transform.position);
            if (objCell == cellPosition)
            {
                Destroy(obj);
            }
        }
    }

    public IEnumerator RebuildFromSave(List<PlacedFurnitureData> items)
    {
        yield return null;
        foreach (var item in items)
        {
            FurnitureSO so = FurnitureDatabase.Instance.GetByID(item.id);
            if(so == null)
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
