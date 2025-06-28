using UnityEngine;

[CreateAssetMenu(fileName = "CardPreviewSettings", menuName = "ScriptableObjects/CardPreviewSettings")]
public class CardPreviewSettings : ScriptableObject
{
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
}
