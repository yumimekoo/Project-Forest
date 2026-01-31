using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BuildMode3D : MonoBehaviour
{
    public DynamicGrid grid;
    public Camera mainCamera;
    public GameObject deletionPreviewPrefab;

    public SoundSO placeSound;
    public SoundSO removeSound;

    private FurnitureSO currentItem;
    private int rotY = 0;
    private GameObject preview;

    private bool isPlacing = false;
    private bool placingMode = false;
    private bool deleteMode = false;
    public bool isPointerOverUI = false;
    private bool previewVisible = true;

    private HashSet<Vector2Int> occupiedCells = new();

    public event System.Action<bool> deleteModeChanged;

    public InputActionAsset InputActions;
    public InputAction rotateAction;
    public InputAction clickAction;
    public InputAction rightClickAction;
    public InputAction pointerPos;
    
    private Renderer[] previewRenderers;
    private MaterialPropertyBlock mpb;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    
    private void Awake()
    {
        clickAction = InputSystem.actions.FindAction("LeftClick");
        rightClickAction = InputSystem.actions.FindAction("DeleteMode");
        rotateAction = InputSystem.actions.FindAction("Rotate");
        pointerPos = InputSystem.actions.FindAction("PointerPos");
    }


    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
        if (FurnitureInventory.Instance) FurnitureInventory.Instance.OnInventoryChanged += HandleInventoryChanged;
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
        if (FurnitureInventory.Instance) FurnitureInventory.Instance.OnInventoryChanged -= HandleInventoryChanged;
    }

    public void StartBuild(FurnitureSO item)
    {
        deleteMode = false;
        placingMode = true;
        currentItem = item;
        isPlacing = true;
        
        RefreshPreviewForCurrentMode();
    }

    public void StopBuild()
    {
        placingMode = false;
        deleteMode = false;
        isPlacing = false;

        SetPreview(null);
    }

    private void HandleInventoryChanged(int id, int newAmount)
    {
        if (!currentItem) return;
        if (deleteMode) return;
        if (currentItem.numericID != id) return;
        if (newAmount <= 0)
        {
            StopBuild();
        }
    }

    private void Update()
    {
        if(!GameState.isInBuildMode)
            return;
        if(rightClickAction.WasPressedThisFrame())
            HandleDeleteToggle();

        if (!isPlacing)
            return;

        if(isPointerOverUI)
            return;

        HandlePlacementOrDeletion();
    }

    public void HandleDeleteToggle()
    {
            deleteMode = !deleteMode;
            deleteModeChanged?.Invoke(deleteMode);

            if (GameState.inTutorial)
            {
                if(TutorialManager.Instance)
                    TutorialManager.Instance.OnDeletionModePressed();
            }

            if (deleteMode)
            {
                isPlacing = true;
                RefreshPreviewForCurrentMode();
                return;
            }
            if (placingMode && currentItem != null)
            {
                RefreshPreviewForCurrentMode();
                return;
            }
            
            isPlacing = false;
            SetPreview(null);
    }
    
    private void SetPreviewVisible(bool visible)
    {
        previewVisible = visible;

        if (preview != null)
            preview.SetActive(visible);
    }

    public int GetOccupiedCells()
    {
        return occupiedCells.Count;
    }
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
            SetPreviewVisible(false);
            return;
        }

        Vector2Int cell = grid.WorldToGrid(hitInfo.point);
        bool inside = grid.IsInsideGrid(cell.x, cell.y);
        bool occupied = occupiedCells.Contains(cell);

        if (!inside)
        {
            SetPreviewVisible(false);
            return;
        }
            
        SetPreviewVisible(true);
        
        Vector3 snapPos = grid.GetWorldPosition(cell.x, cell.y);

        if (deleteMode)
        {
            Preview(cell, snapPos, occupied);
            if (clickAction.WasPressedThisFrame())
            {
                TryDelete(cell);
            }
            return;
        }

        if (placingMode)
        {
            Preview(cell, snapPos, occupied);

            if(clickAction.WasPressedThisFrame() && !occupied)
            {
                PlaceFurniture(cell, snapPos);
            }
        }
    }
    
    private void SetPreview(GameObject prefab)
    {
        if (preview != null)
            Destroy(preview);

        if (prefab != null)
        {
            preview = Instantiate(prefab);
            preview.tag = "Untagged";
            preview.layer = LayerMask.NameToLayer("Ignore Raycast");
            
            previewRenderers = preview.GetComponentsInChildren<Renderer>(true);
            if(mpb == null) mpb = new MaterialPropertyBlock();
        }
        else
        {
            preview = null;
            previewRenderers = null;
        }
    }

    private void RefreshPreviewForCurrentMode()
    {
        // Delete Mode -> fixed prefab
        if (deleteMode)
        {
            SetPreview(deletionPreviewPrefab);
            return;
        }

        // Place Mode -> furniture preview vom ausgewählten Item
        if (currentItem != null && currentItem.furniturePreview != null)
        {
            SetPreview(currentItem.furniturePreview);
            return;
        }

        // Fallback: kein Preview
        SetPreview(null);
    }
    
    private void Preview(Vector2Int cell, Vector3 snapPos, bool occupied)
    {
        if (preview == null)
            return;

        preview.transform.position = snapPos;
        preview.transform.rotation = Quaternion.Euler(0, rotY, 0);
        if (deleteMode)
        {
            return;
        }
        
        SetPreviewColor(occupied
            ? new Color(1, 0, 0, 0.3f)
            : new Color(0, 1, 0, 0.3f));
        
    }
    
    private void SetPreviewColor(Color c)
    {
        if (previewRenderers == null) return;

        mpb.Clear();
        mpb.SetColor(BaseColorID, c);

        foreach (var r in previewRenderers)
            r.SetPropertyBlock(mpb);
    }
    
    private void PlaceFurniture(Vector2Int cell, Vector3 position)
    {
        if(!FurnitureInventory.Instance.Remove(currentItem.numericID))
        {
            return;
        }
        var go = Instantiate(currentItem.furniturePrefab, position, Quaternion.Euler(0, rotY, 0));
        go.AddComponent<FurnitureIdentifier>().so = currentItem;
        occupiedCells.Add(cell);
        AudioManager.Instance.PlayAt(placeSound, position);
        FurniturePlacementManager.Instance.RegisterPlacement(currentItem.numericID, cell, rotY);

        if(GameState.inTutorial)
        {
            if(TutorialManager.Instance != null)
                TutorialManager.Instance.OnObjectPlaced();
        }

    }

    private void TryRandomPlace(FurnitureSO item)
    {
        List<Vector2Int> freeCells = new List<Vector2Int>();
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (!occupiedCells.Contains(cell))
                {
                    freeCells.Add(cell);
                }
            }
        }
        if (freeCells.Count == 0)
        {
            return;
        }
        Vector2Int randomCell = freeCells[Random.Range(0, freeCells.Count)];
        Vector3 position = grid.GetWorldPosition(randomCell.x, randomCell.y);
        var go = Instantiate(item.furniturePrefab, position, Quaternion.Euler(0, 0, 0));
        go.AddComponent<FurnitureIdentifier>().so = item;
        occupiedCells.Add(randomCell);
        FurniturePlacementManager.Instance.RegisterPlacement(item.numericID, randomCell, 0);
        FurnitureInventory.Instance.Remove(item.numericID);
        //Debug.Log($"Randomly placed furniture: {item.furnitureName} (ID: {item.numericID}) at ({randomCell.x}, {randomCell.y})");
    }

    private void TryDelete(Vector2Int cell)
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Furniture"))
        {
            if (grid.WorldToGrid(obj.transform.position) != cell)
                continue;

            var ident = obj.GetComponent<FurnitureIdentifier>();
            if (ident == null || ident.so == null)
            {
                Debug.LogWarning($"[BuildMode3D] Objekt '{obj.name}' hat Tag 'Furniture', aber keinen FurnitureIdentifier/so.");
                continue;
            }

            var soItem = ident.so;

            if (FurnitureInventory.Instance != null)
                FurnitureInventory.Instance.Add(soItem.numericID);

            AudioManager.Instance?.PlayAt(removeSound, obj.transform.position);

            Destroy(obj);

            occupiedCells.Remove(cell);
            FurniturePlacementManager.Instance?.RemovePlacement(cell);

            if (GameState.inTutorial && TutorialManager.Instance != null)
                TutorialManager.Instance.OnObjectDeleted();

            return;
        }
    }
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
            //Debug.Log($"Rebuilt furniture: {so.furnitureName} (ID: {so.numericID}) at ({item.x}, {item.y}) with rotation {item.rotY}");
        }
    }
    public void RandomizeGrid()
    {
        StartCoroutine(RandomPlacer());
    }

    private IEnumerator RandomPlacer()
    {
        yield return null;

        Dictionary<int, int> inventoryItems = new Dictionary<int, int>(FurnitureInventory.Instance.GetAll());

        foreach (var entry in inventoryItems)
        {
            FurnitureSO so = FurnitureDatabase.Instance.GetByID(entry.Key);
            if (so == null)
                continue;

            for(int i = 0; i < entry.Value; i++)
            {
                TryRandomPlace(so);
            }
        }
    }
}
