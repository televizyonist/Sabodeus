using UnityEngine;

public class HandLayoutFanStyle : MonoBehaviour
{
    // Wider spacing to reduce overlap
    public float spacing = 1.25f;
    // Slightly flatter curve for a smoother fan
    public float curveHeight = 0.2f;
    public float rotationScale = 5f;
    // Keep cards one unit apart on the Z axis
    public float depthOffset = 1f;
    public float verticalShift = 0f;

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
