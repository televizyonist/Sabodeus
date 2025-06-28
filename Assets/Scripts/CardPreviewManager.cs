using UnityEngine;

public class CardPreviewManager : MonoBehaviour
{
    public static CardPreviewManager Instance { get; private set; }

    [Header("Preview Settings")]
    [Tooltip("World position used when 'useScreenPosition' is disabled")]
    public Vector3 previewPosition = Vector3.zero;

    [Tooltip("Normalized screen position (Viewport space) where the preview will appear")] 
    public Vector2 previewScreenPosition = new Vector2(0.5f, 0.5f);

    [Tooltip("Distance from the camera when using screen position")] 
    public float previewDepth = 2f;

    [Tooltip("If true previewScreenPosition is used to place the preview")] 
    public bool useScreenPosition = false;

    public float previewScale = 0.7f;

    private GameObject currentPreview;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        var go = new GameObject("CardPreviewManager");
        go.AddComponent<CardPreviewManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPreview(GameObject card)
    {
        HidePreview();
        Vector3 spawnPos = previewPosition;
        if (useScreenPosition && Camera.main != null)
        {
            Vector3 viewportPos = new Vector3(previewScreenPosition.x, previewScreenPosition.y, previewDepth);
            spawnPos = Camera.main.ViewportToWorldPoint(viewportPos);
        }
        currentPreview = Instantiate(card, spawnPos, Quaternion.identity);
        foreach (var drag in currentPreview.GetComponentsInChildren<CardDraggable>(true))
            Destroy(drag);
        foreach (var col in currentPreview.GetComponentsInChildren<Collider>(true))
            Destroy(col);
        currentPreview.transform.localScale = Vector3.one * previewScale;
    }

    public void HidePreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }
}
