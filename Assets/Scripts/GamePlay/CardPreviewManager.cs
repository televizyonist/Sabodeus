using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardPreviewManager : NetworkBehaviour
{
    public static CardPreviewManager Instance { get; private set; }

    [Tooltip("Settings asset used for preview parameters")]
    public CardPreviewSettings settings;

    private GameObject currentPreview;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        // Sadece GamePlay sahnesinde çalýþsýn
        if (SceneManager.GetActiveScene().name != "GamePlay")
            return;

        if (Instance != null)
            return;

        var go = new GameObject("CardPreviewManager");
        go.AddComponent<CardPreviewManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "GamePlay")
        {
            Destroy(gameObject);
            return;
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (settings == null)
            settings = Resources.Load<CardPreviewSettings>("CardPreviewSettings");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ShowPreview(GameObject card)
    {
        HidePreview();
        Vector3 spawnPos = settings != null ? settings.previewPosition : Vector3.zero;
        bool useScreenPosition = settings != null && settings.useScreenPosition;
        Vector2 previewScreenPosition = settings != null ? settings.previewScreenPosition : new Vector2(0.5f, 0.5f);
        float previewDepth = settings != null ? settings.previewDepth : 2f;
        float previewScale = settings != null ? settings.previewScale : 0.7f;

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
