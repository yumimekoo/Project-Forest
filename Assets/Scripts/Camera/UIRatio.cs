using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIRatio : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    // Empfohlen: im UXML einen Wrapper anlegen: <VisualElement name="viewportRoot" ... />
    // Dann hier "viewportRoot" eintragen.
    [SerializeField] private string viewportRootName = "rootScale";

    private UIDocument doc;
    private VisualElement viewportRoot;

    private Rect lastCamRect;
    private int lastW, lastH;

    private void Awake()
    {
        doc = GetComponent<UIDocument>();
        if (!targetCamera) targetCamera = Camera.main;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(GameState.inTutorial)
            targetCamera = Camera.main;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;   
        StartCoroutine(InitWhenPanelReady());
    }

    private IEnumerator InitWhenPanelReady()
    {
        // Warten bis Panel existiert (sehr wichtig!)
        while (doc.rootVisualElement.panel == null)
            yield return null;

        viewportRoot = string.IsNullOrEmpty(viewportRootName)
            ? doc.rootVisualElement
            : (doc.rootVisualElement.Q<VisualElement>(viewportRootName) ?? doc.rootVisualElement);

        viewportRoot.style.position = Position.Absolute;
        viewportRoot.style.overflow = Overflow.Hidden;

        Apply();
    }

    private void Update()
    {
        if (doc.rootVisualElement.panel == null) return;

        if (Screen.width != lastW || Screen.height != lastH || targetCamera.rect != lastCamRect)
            Apply();
    }

    private void Apply()
    {
        lastW = Screen.width;
        lastH = Screen.height;
        lastCamRect = targetCamera.rect;

        var panel = doc.rootVisualElement.panel;

        // Camera.rect ist normalized (0..1) im Screen, Ursprung unten-links.
        Rect r = targetCamera.rect;

        Vector2 screenBL = new Vector2(r.xMin * Screen.width, r.yMin * Screen.height); // bottom-left
        Vector2 screenTR = new Vector2(r.xMax * Screen.width, r.yMax * Screen.height); // top-right

        // In Panel-Koordinaten umrechnen (UI Toolkit Ursprung oben-links)
        Vector2 panelBL = RuntimePanelUtils.ScreenToPanel(panel, screenBL);
        Vector2 panelTR = RuntimePanelUtils.ScreenToPanel(panel, screenTR);

        float left = panelBL.x;
        float top = panelTR.y; // top-left y kommt von TR.y (weil panel y nach unten wächst)

        float width = panelTR.x - panelBL.x;
        float height = panelBL.y - panelTR.y;

        // Safety (falls negative durch rounding)
        if (width < 0) { left += width; width = -width; }
        if (height < 0) { top += height; height = -height; }

        viewportRoot.style.left = left;
        viewportRoot.style.top = top;
        viewportRoot.style.width = width;
        viewportRoot.style.height = height;
    }
    
}
