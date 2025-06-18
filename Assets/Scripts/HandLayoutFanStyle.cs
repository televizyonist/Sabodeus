using UnityEngine;

public class HandLayoutFanStyle : MonoBehaviour
{
    public float radius = 2f;
    public float angleSpread = 15f;
    public float depthOffset = 0.01f;
    public float verticalShift = 0f;

    public void UpdateLayout()
    {
        int count = transform.childCount;
        if (count == 0) return;

        float maxAngle = Mathf.Min(angleSpread * (count - 1), 60f);
        float step = count > 1 ? maxAngle / (count - 1) : 0f;
        float startAngle = -maxAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            Transform card = transform.GetChild(i);

            float angle = startAngle + step * i;

            Vector3 pos = Quaternion.Euler(0f, 0f, angle) * Vector3.up * radius;
            pos.y -= radius;
            pos.y += verticalShift;

            card.localPosition = new Vector3(pos.x, pos.y, -i * depthOffset);
            card.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
