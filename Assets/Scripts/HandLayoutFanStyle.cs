using UnityEngine;

public class HandLayoutFanStyle : MonoBehaviour
{
    // Wider spacing so cards barely overlap
    public float spacing = 1.75f;
    // Higher curve so cards stay visible
    public float curveHeight = 0.3f;
    // Slightly stronger rotation for a fuller fan
    public float rotationScale = 8f;
    // Keep cards one unit apart on the Z axis
    public float depthOffset = 1f;
    // Raise the whole hand so cards are fully visible
    public float verticalShift = 1.5f;

    public void UpdateLayout()
    {
        int count = transform.childCount;
        if (count == 0) return;
        for (int i = 0; i < count; i++)
        {
            Transform card = transform.GetChild(i);
            float offsetIndex = i - (count - 1) / 2f;
            float x = offsetIndex * spacing;
            float y = -Mathf.Abs(offsetIndex) * curveHeight + verticalShift;

            card.localPosition = new Vector3(x, y, -i * depthOffset);
            float angle = offsetIndex * rotationScale;
            card.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
