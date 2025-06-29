using Unity.Netcode;
using UnityEngine;

public class HandLayoutFanStyle : NetworkBehaviour
{
    // Wider spacing to reduce overlap
    public float spacing = 1.25f;
    // Maximum width the fan layout should occupy
    public float maxWidth = 5f;
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
    private Coroutine _layoutRoutine;
    public bool animateLayout = true;
    public float layoutAnimationDuration = 0.2f;

    private void Awake()
    {
        _baseSpacing = spacing;
        _currentSpacing = spacing;
    }

    public void UpdateLayout()
    {
        _baseSpacing = CalculateBaseSpacing();
        _currentSpacing = _baseSpacing;

        if (_layoutRoutine != null)
            StopCoroutine(_layoutRoutine);

        if (animateLayout)
            _layoutRoutine = StartCoroutine(AnimateLayout());
        else
            ApplyLayout();
    }

    private float CalculateBaseSpacing()
    {
        int count = transform.childCount;
        if (maxWidth > 0f && count > 1)
            return Mathf.Min(spacing, maxWidth / (count - 1));
        return spacing;
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

    private System.Collections.IEnumerator AnimateLayout()
    {
        int count = transform.childCount;
        if (count == 0)
        {
            _layoutRoutine = null;
            yield break;
        }

        Transform[] cards = new Transform[count];
        Vector3[] startPos = new Vector3[count];
        Quaternion[] startRot = new Quaternion[count];
        Vector3[] endPos = new Vector3[count];
        Quaternion[] endRot = new Quaternion[count];

        for (int i = 0; i < count; i++)
        {
            Transform card = transform.GetChild(i);
            cards[i] = card;
            startPos[i] = card.localPosition;
            startRot[i] = card.localRotation;

            float offsetIndex = i - (count - 1) / 2f;
            float x = offsetIndex * _currentSpacing;
            float y = -Mathf.Abs(offsetIndex) * curveHeight + verticalShift;
            endPos[i] = new Vector3(x, y, -i * depthOffset);
            float angle = offsetIndex * rotationScale;
            endRot[i] = Quaternion.Euler(0, 0, angle);
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, layoutAnimationDuration);
            for (int i = 0; i < count; i++)
            {
                cards[i].localPosition = Vector3.Lerp(startPos[i], endPos[i], t);
                cards[i].localRotation = Quaternion.Lerp(startRot[i], endRot[i], t);
            }
            yield return null;
        }

        for (int i = 0; i < count; i++)
        {
            cards[i].localPosition = endPos[i];
            cards[i].localRotation = endRot[i];
        }

        _layoutRoutine = null;
    }
}
