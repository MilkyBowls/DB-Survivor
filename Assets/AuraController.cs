using System.Collections;
using UnityEngine;

public class AuraController : MonoBehaviour
{
    public SpriteRenderer auraRenderer;
    public float pulseSpeed = 2f;
    public float pulseScaleAmount = 0.1f;
    public float pulseAlphaAmount = 0.2f;

    public Vector2 idleOffset = Vector2.zero;
    public Vector2 walkOffset = Vector2.zero;

    private Vector3 defaultScale;
    private Vector3 defaultLocalPosition;
    private Color baseColor;
    private bool isActive = false;
    private Vector3 currentScale;

    void Start()
    {
        if (auraRenderer != null)
        {
            baseColor = auraRenderer.color;
        }

        defaultScale = transform.localScale;
        defaultLocalPosition = transform.localPosition;
    }

    void Update()
    {
        if (!isActive || auraRenderer == null) return;

        // Pulsing scale added to current base scale (which includes rotation/flip)
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseScaleAmount;
        transform.localScale = currentScale + Vector3.one * pulse;

        // Pulsing alpha
        Color c = baseColor;
        c.a = baseColor.a - Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed) * pulseAlphaAmount);
        auraRenderer.color = c;
    }

    public void SetAura(Sprite auraSprite)
    {
        if (auraRenderer != null)
        {
            auraRenderer.sprite = auraSprite;
            baseColor = auraRenderer.color;
        }
    }

    public void EnableAura()
    {
        if (auraRenderer != null)
        {
            auraRenderer.enabled = true;
            isActive = true;
        }
    }

    public void DisableAura()
    {
        if (auraRenderer != null)
        {
            auraRenderer.enabled = false;
            isActive = false;
            transform.localScale = defaultScale;
            auraRenderer.color = baseColor;
        }
    }

    public void FadeInAura(float duration = 0.5f)
    {
        if (auraRenderer == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(duration));
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        isActive = true;
        auraRenderer.enabled = true;

        Color c = auraRenderer.color;
        c.a = 0f;
        auraRenderer.color = c;

        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            c.a = Mathf.Lerp(0f, baseColor.a, t);
            auraRenderer.color = c;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        c.a = baseColor.a;
        auraRenderer.color = c;
    }

    public void UpdateAuraOrientation(string animationStateName, bool faceLeft)
    {
        if (auraRenderer == null) return;

        switch (animationStateName)
        {
            case "Walk":
            case "WalkAttack":
                transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

                // Flip Y when rotated sideways
                Vector3 walkScale = defaultScale;
                walkScale.y = Mathf.Abs(walkScale.y) * (faceLeft ? -1f : 1f);
                currentScale = walkScale;

                Vector2 walkOffsetFlipped = walkOffset;
                if (faceLeft) walkOffsetFlipped.x *= -1f;

                transform.localPosition = defaultLocalPosition + (Vector3)walkOffsetFlipped;
                break;

            default:
                transform.localRotation = Quaternion.identity;

                Vector3 resetScale = defaultScale;
                resetScale.x = Mathf.Abs(resetScale.x);
                resetScale.y = Mathf.Abs(resetScale.y); // always positive
                currentScale = resetScale;

                Vector2 idleOffsetFlipped = idleOffset;
                if (faceLeft) idleOffsetFlipped.x *= -1f;
                transform.localPosition = defaultLocalPosition + (Vector3)idleOffsetFlipped;

                transform.localRotation = Quaternion.identity;
                break;
        }
    }
}
