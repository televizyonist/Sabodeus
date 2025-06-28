using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Card Preview Settings")]
public class CardPreviewSettings : ScriptableObject
{
    public Vector3 previewPosition = Vector3.zero;
    public Vector2 previewScreenPosition = new Vector2(0.5f, 0.5f);
    public float previewDepth = 2f;
    public bool useScreenPosition = false;
    public float previewScale = 0.7f;
}
