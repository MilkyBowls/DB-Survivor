using System.Collections;
using UnityEngine;

public class AuraController : MonoBehaviour
{
    public Animator auraAnimator;
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
            baseColor = auraRenderer.color;

        defaultScale = transform.localScale;
        defaultLocalPosition = transform.localPosition;
    }

    void Update()
    {
        if (!isActive || auraRenderer == null || isCharging) return;

        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseScaleAmount;
        transform.localScale = currentScale + Vector3.one * pulse;

        Color c = baseColor;
        c.a = baseColor.a - Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed) * pulseAlphaAmount);
        auraRenderer.color = c;
    }

    public void EnableAura()
    {
        if (auraAnimator != null)
        {
            auraAnimator.enabled = true;
            auraAnimator.gameObject.SetActive(true); // Optional but safe
            auraAnimator.SetBool("IsActive", true);
        }

        if (auraRenderer != null)
        {
            auraRenderer.enabled = true;
            isActive = true;
        }
    }

    public void DisableAura()
    {
        if (auraAnimator != null)
        {
            auraAnimator.SetBool("IsActive", false);
            auraAnimator.SetBool("IsCharging", false);
            auraAnimator.enabled = false;
        }

        if (auraRenderer != null)
        {
            auraRenderer.enabled = false;
            auraRenderer.color = baseColor;
        }

        isActive = false;
        transform.localScale = defaultScale;
    }


    private bool isCharging = false;

    public void PlayChargeAnimation(bool charging)
    {
        Debug.Log($"Aura PlayChargeAnimation({charging}) | Animator: {auraAnimator}, Active: {isActive}");

        if (auraAnimator != null && isActive)
        {
            isCharging = charging;
            auraAnimator.SetBool("IsCharging", charging);
        }
    }

    public void SetAura(Sprite auraSprite) { }


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
                resetScale.y = Mathf.Abs(resetScale.y);
                currentScale = resetScale;

                Vector2 idleOffsetFlipped = idleOffset;
                if (faceLeft) idleOffsetFlipped.x *= -1f;
                transform.localPosition = defaultLocalPosition + (Vector3)idleOffsetFlipped;
                break;
        }
    }
}
