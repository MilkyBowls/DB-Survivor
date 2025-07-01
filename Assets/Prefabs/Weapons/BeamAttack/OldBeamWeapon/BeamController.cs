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

    public void SetBeamVisuals(float length, float startEndScale, float midScaleX, float midScaleY, float midBaseHeight)
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

        // Set Z-depth order manually
        startTransform.localPosition = new Vector3(0f, 0f, 0.01f);
        midRenderer.transform.localPosition = new Vector3(startWidth + buffer, 0f, 0.02f);
        endTransform.localPosition = new Vector3(startWidth + buffer + currentMidLength, 0f, 0.01f);

        // Apply scale
        startTransform.localScale = Vector3.one * startEndScale;
        endTransform.localScale = Vector3.one * startEndScale;

        if (midRenderer != null)
        {
            float midHeight = midBaseHeight * midScaleY;
            midRenderer.size = new Vector2(currentMidLength * midScaleX, midHeight);
        }

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

    public void ApplyVisualOverrides(AnimatorOverrideController startOverride, AnimatorOverrideController endOverride, Sprite midOverride)
    {
        if (startTransform != null && startTransform.TryGetComponent(out Animator startAnim) && startOverride != null)
            startAnim.runtimeAnimatorController = startOverride;

        if (endTransform != null && endTransform.TryGetComponent(out Animator endAnim) && endOverride != null)
            endAnim.runtimeAnimatorController = endOverride;

        if (midRenderer != null && midOverride != null)
            midRenderer.sprite = midOverride;
    }

    public void SetSortingOrder(int beamGroupIndex, int baseOrder = 0)
    {
        int groupOrder = baseOrder + (beamGroupIndex * 10); // ← space out more

        if (startTransform.TryGetComponent(out SpriteRenderer startSR))
            startSR.sortingOrder = groupOrder + 1;

        if (midRenderer != null)
            midRenderer.sortingOrder = groupOrder;

        if (endTransform.TryGetComponent(out SpriteRenderer endSR))
            endSR.sortingOrder = groupOrder + 1;
    }
}