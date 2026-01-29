using UnityEngine;

public class NPCView : MonoBehaviour
{
    public Transform visualContainer;
    public GameObject currentVisual;

    public void SetVisual(GameObject visualPrefab)
    {
        if(currentVisual) Destroy(currentVisual);

        if (!visualContainer || !visualPrefab)
        {
            currentVisual = null;
            return;
        }
        
        currentVisual = Instantiate(visualPrefab, visualContainer);
        currentVisual.transform.localPosition = Vector3.zero;
        currentVisual.transform.localRotation = Quaternion.identity;
        currentVisual.transform.localScale = Vector3.one;
    }
}
