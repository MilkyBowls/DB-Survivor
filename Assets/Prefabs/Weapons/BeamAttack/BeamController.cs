using UnityEngine;

public class BeamController : MonoBehaviour
{
    [Header("Beam Parts")]
    public Transform startTransform;
    public SpriteRenderer midRenderer;
    public Transform endTransform;
    public BoxCollider2D damageCollider;

    [Header("Visual Root")]
    [SerializeField] public Transform visualRoot;

    private float currentMidLength = 0f;
    private float beamWidth = 1f;

    public void SetBeamWidth(float width)
    {
        beamWidth = width;
    }

    public void SetStartEndScale(float scale)
    {
        if (startTransform != null)
            startTransform.localScale = Vector3.one * scale;

        if (endTransform != null)
            endTransform.localScale = Vector3.one * scale;
    }

public void SetBeamVisuals(float length, float startEndScale, float midScaleX, float midScaleY)
{
    if (startTransform == null || midRenderer == null || endTransform == null)
        return;

    float pixelsPerUnit = 16f;
    float startWidth = startTransform.GetComponent<SpriteRenderer>().sprite.rect.width / pixelsPerUnit;
    float endWidth = endTransform.GetComponent<SpriteRenderer>().sprite.rect.width / pixelsPerUnit;
    float buffer = 0f;

    float minLength = startWidth + endWidth + buffer;
    length = Mathf.Max(length, minLength);

    currentMidLength = Mathf.Max(0.01f, length - (startWidth + endWidth + buffer));

    startTransform.localPosition = Vector3.zero;
    midRenderer.transform.localPosition = new Vector3(startWidth + buffer, 0f, 0f);
    endTransform.localPosition = new Vector3(startWidth + buffer + currentMidLength, 0f, 0f);

    // Apply scale
    if (startTransform != null)
        startTransform.localScale = Vector3.one * startEndScale;

    if (endTransform != null)
        endTransform.localScale = Vector3.one * startEndScale;

    if (midRenderer != null)
        midRenderer.size = new Vector2(currentMidLength * midScaleX, beamWidth * midScaleY);

    if (damageCollider != null)
    {
        damageCollider.offset = new Vector2(startWidth + buffer + (currentMidLength * 0.5f), 0f);
        damageCollider.size = new Vector2(currentMidLength, beamWidth * midScaleY);
    }

    Debug.Log($"[BeamController] SetBeamVisuals | length={length:F2}, midSize=({currentMidLength * midScaleX}, {beamWidth * midScaleY})");
}


    public void SetDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}