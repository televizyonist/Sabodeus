using UnityEngine;

public class CardPreviewManager : MonoBehaviour
{
    public static CardPreviewManager Instance { get; private set; }

    [Header("Preview Settings")]
    public Vector3 previewPosition = Vector3.zero;
    public float previewScale = 3f;

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
        currentPreview = Instantiate(card, previewPosition, Quaternion.identity);
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
