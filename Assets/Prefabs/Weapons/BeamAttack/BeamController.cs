using System.Collections;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    [Header("Beam Parts")]
    public Transform startTransform;
    public SpriteRenderer midRenderer;
    public Transform endTransform;
    public BoxCollider2D damageCollider;

    [Header("Beam Settings")]
    public float beamWidth = 1f;

    [SerializeField] private AnimationCurve growthCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    public void SetBeamLength(float length)
    {
        if (startTransform == null || midRenderer == null || endTransform == null)
            return;

        float startWidth = startTransform.GetComponent<SpriteRenderer>().sprite.rect.width / 16f;
        float endWidth = endTransform.GetComponent<SpriteRenderer>().sprite.rect.width / 16f;

        // Clamp total beam length to at least (start + end)
        length = Mathf.Max(startWidth + endWidth, length);

        // Clamp mid section to minimum size (even if beam is very short)
        float rawMid = length - (startWidth + endWidth);
        float midLength = Mathf.Max(1f, rawMid); // ✅ force minimum mid

        // Position start
        startTransform.localPosition = Vector3.zero;

        // Position and size mid
        midRenderer.transform.localPosition = new Vector3(startWidth, 0f, 0f);
        midRenderer.size = new Vector2(midLength, beamWidth);

        // Position end
        endTransform.localPosition = new Vector3(startWidth + midLength, 0f, 0f);

        // Collider
        if (damageCollider != null)
        {
            damageCollider.offset = new Vector2(startWidth + midLength * 0.5f, 0f);
            damageCollider.size = new Vector2(midLength, beamWidth);
        }
    }





    public void SetDirection(Vector2 direction)
    {
        Debug.Log($"[BeamController] SetDirection({direction})");

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void AnimateBeamGrowth(float targetLength, float growDuration)
    {
        StartCoroutine(GrowBeam(targetLength, growDuration));
    }

    private IEnumerator GrowBeam(float targetLength, float duration)
    {
        float startWidth = 1f; // match BeamStart width
        float endWidth = 1f;   // match BeamEnd width
        float minLength = startWidth + endWidth + 0.1f;

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            float eased = growthCurve.Evaluate(t);
            float currentLength = Mathf.Lerp(minLength, targetLength, eased);

            SetBeamLength(currentLength);
            time += Time.deltaTime;
            yield return null;
        }

        SetBeamLength(targetLength); // final snap to ensure precision
    }
}
