using UnityEngine;

public class HandLayoutFanStyle : MonoBehaviour
{
    public float radius = 2f;
    public float angleSpread = 15f;
    public float depthOffset = 0.01f;

    public void UpdateLayout()
    {
        int count = transform.childCount;
        if (count == 0) return;

        float totalAngle = Mathf.Min(angleSpread * (count - 1), 60f);
        float startAngle = -totalAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            Transform card = transform.GetChild(i);

            float angle = startAngle + angleSpread * i;
            float rad = Mathf.Deg2Rad * angle;

            float x = Mathf.Sin(rad) * radius;
            float y = Mathf.Cos(rad) * radius;

            card.localPosition = new Vector3(x, y, -i * depthOffset);
            card.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
