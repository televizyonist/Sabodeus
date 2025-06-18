using UnityEngine;

public class HandLayoutFanStyle : MonoBehaviour
{
    public float spacing = 1f;
    public float curveHeight = 0.3f;
    public float rotationScale = 5f;
    public float depthOffset = 0.01f;
    public float verticalShift = 0f;

    public void UpdateLayout()
    {
        int count = transform.childCount;
        if (count == 0) return;
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
        for (int i = 0; i < count; i++)
        {
            Transform card = transform.GetChild(i);
            float x = (i - (count - 1) / 2f) * spacing;
            float y = -Mathf.Abs(x) * curveHeight + verticalShift;

            card.localPosition = new Vector3(x, y, -i * depthOffset);
            float angle = x * rotationScale;
            card.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}