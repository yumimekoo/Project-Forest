using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildStarter : MonoBehaviour
{
    public BuildMode3D buildMode;
    public CameraFollow cameraFollow;
    public FurnitureSO item1;
    public FurnitureSO item2;
    public FurnitureSO item3;
    public InputActionAsset InputActions;
    public InputAction item1action;
    public InputAction item2action;
    public InputAction item3action;
    public InputAction escape;
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
        item1action = InputSystem.actions.FindAction("Item1");
        item2action = InputSystem.actions.FindAction("Item2");
        item3action = InputSystem.actions.FindAction("Item3");
        escape = InputSystem.actions.FindAction("Delete");
    }
    private void Update()
    {
        return;
        if(item1action.WasPressedThisFrame())
        {
            buildMode.StartBuild(item1);
            cameraFollow.ChangeFollowTarget(GameState.isInBuildMode);
        }
        if (item2action.WasPressedThisFrame())
        {
            buildMode.StartBuild(item2);
            cameraFollow.ChangeFollowTarget(GameState.isInBuildMode);

        }
        if (item3action.WasPressedThisFrame())
        {
            buildMode.StartBuild(item3);
            cameraFollow.ChangeFollowTarget(GameState.isInBuildMode);

        }
        if (escape.WasPressedThisFrame())
        {
            buildMode.StopBuild();
            cameraFollow.ChangeFollowTarget(GameState.isInBuildMode);

        }
    }
}
