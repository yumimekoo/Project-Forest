using UnityEngine;

public class AspectRatio : MonoBehaviour
{
    private Camera cam;
    private const float targetAspect = 16f / 9f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        Apply();
    }

    private void OnEnable() => Apply();

    private void Update()
    {
        Apply(); // optional nur bei Resize optimieren
    }

    private void Apply()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
        {
            // Pillarbox (links/rechts)

            Rect rect = cam.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;
            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;
            cam.rect = rect;
            // Letterbox (oben/unten)

        }
    }
}
