using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] UIDocument interactionUI;

    public static InteractionUI Instance;
    private Label interactionLabel;
    private VisualElement root;

    private void Awake()
    {
        Instance = this;

        root = interactionUI.rootVisualElement;
        interactionLabel = root.Q<Label>("interactionLabel");

        HideUI();
    }

    private void LateUpdate()
    {
        if (root.style.display == DisplayStyle.Flex)
        {
            Vector3 dir = transform.position - Camera.main.transform.position;
            if(!GameState.isInRoom) dir.x = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    public void Show(string text, Vector3 position)
    {
        if(text == null) return;
        interactionLabel.text = text;
        transform.position = position;
        ShowUI();
    }

    public void HideUI()
    {
        root.style.display = DisplayStyle.None;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
    }
    public void ShowUI() => root.style.display = DisplayStyle.Flex;
}
