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

    [Header("Hover Spread Settings")]
    public float spreadMultiplier = 1.1f; // how much to spread when hovering
    public float spreadDuration = 0.2f;   // duration of the spread animation

    private float _baseSpacing;
    private float _currentSpacing;
    private Coroutine _spreadRoutine;

    private void Awake()
    {
        _baseSpacing = spacing;
        _currentSpacing = spacing;
    }

    public void UpdateLayout()
    {
        _baseSpacing = spacing;
        _currentSpacing = spacing;
        ApplyLayout();
    }

    private void ApplyLayout()
    {
        int count = transform.childCount;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            Transform card = transform.GetChild(i);
            float offsetIndex = i - (count - 1) / 2f;
            float x = offsetIndex * _currentSpacing;
            float y = -Mathf.Abs(offsetIndex) * curveHeight + verticalShift;

            card.localPosition = new Vector3(x, y, -i * depthOffset);
            float angle = offsetIndex * rotationScale;
            card.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private System.Collections.IEnumerator AnimateSpacing(float target)
    {
        float start = _currentSpacing;
        float time = 0f;
        while (time < spreadDuration)
        {
            time += Time.deltaTime;
            _currentSpacing = Mathf.Lerp(start, target, time / spreadDuration);
            ApplyLayout();
            yield return null;
        }
        _currentSpacing = target;
        ApplyLayout();
        _spreadRoutine = null;
    }

    public void SpreadOut()
    {
        if (_spreadRoutine != null) StopCoroutine(_spreadRoutine);
        _spreadRoutine = StartCoroutine(AnimateSpacing(_baseSpacing * spreadMultiplier));
    }

    public void Restore()
    {
        if (_spreadRoutine != null) StopCoroutine(_spreadRoutine);
        _spreadRoutine = StartCoroutine(AnimateSpacing(_baseSpacing));
    }
}
