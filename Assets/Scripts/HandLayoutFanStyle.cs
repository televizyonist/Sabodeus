using UnityEngine;

public class HandLayoutFanStyle : MonoBehaviour
{
    // Wider spacing so cards don't overlap
    public float spacing = 1.5f;
    // Higher curve to keep cards fully visible
    public float curveHeight = 0.25f;
    // Rotation independent from spacing for a consistent fan
    public float rotationScale = 7f;
    // Keep cards one unit apart on the Z axis
    public float depthOffset = 1f;
    // Raise the whole hand to fit on screen
    public float verticalShift = 1f;

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
