using System.Collections;
using System.Collections.Generic;
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

    [Header("Charging Particle Effects")]
    public List<ParticleSystem> chargeParticlePrefabs;
    public Transform particleSpawnArea;
    public float particlesPerSecond = 4f;
    public Vector2 spawnRadius = new Vector2(0.5f, 0.5f);
    public Vector2 upwardVelocityRange = new Vector2(0.5f, 1.5f);
    public float fadeDuration = 0.5f;

    private Coroutine particleLoopCoroutine;
    private bool isCharging = false;

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
            auraAnimator.gameObject.SetActive(true);
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

        if (particleLoopCoroutine != null)
        {
            StopCoroutine(particleLoopCoroutine);
            particleLoopCoroutine = null;
        }

        isActive = false;
        isCharging = false;
        transform.localScale = defaultScale;
    }

    public void PlayChargeAnimation(bool charging)
    {
        Debug.Log($"Aura PlayChargeAnimation({charging}) | Animator: {auraAnimator}, Active: {isActive}");

        if (auraAnimator != null && isActive)
        {
            isCharging = charging;
            auraAnimator.SetBool("IsCharging", charging);
        }

        if (charging)
        {
            if (particleLoopCoroutine == null)
                particleLoopCoroutine = StartCoroutine(LoopChargeParticles());
        }
        else
        {
            if (particleLoopCoroutine != null)
            {
                StopCoroutine(particleLoopCoroutine);
                particleLoopCoroutine = null;
            }
        }
    }

    private IEnumerator LoopChargeParticles()
    {
        while (true)
        {
            SpawnChargeParticle();
            yield return new WaitForSeconds(1f / particlesPerSecond);
        }
    }

    private void SpawnChargeParticle()
    {
        if (chargeParticlePrefabs == null || chargeParticlePrefabs.Count == 0) return;

        int index = Random.Range(0, chargeParticlePrefabs.Count);
        ParticleSystem prefab = chargeParticlePrefabs[index];

        Vector3 spawnOrigin = particleSpawnArea != null ? particleSpawnArea.position : transform.position;
        Vector2 offset = new Vector2(
            Random.Range(-spawnRadius.x, spawnRadius.x),
            Random.Range(-spawnRadius.y, spawnRadius.y)
        );

        Vector3 spawnPos = spawnOrigin + (Vector3)offset;
        ParticleSystem ps = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

        // Optional Rigidbody2D motion
        Rigidbody2D rb = ps.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float upwardForce = Random.Range(upwardVelocityRange.x, upwardVelocityRange.y);
            rb.linearVelocity = new Vector2(0f, upwardForce);
        }

        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f)); // optional gradient

        ps.Play();
        StartCoroutine(FadeOutAndDestroy(ps, ps.main.duration + ps.main.startLifetime.constantMax));
    }

    private IEnumerator FadeOutAndDestroy(ParticleSystem ps, float totalDuration)
    {
        float timer = 0f;
        float fadeStart = totalDuration - fadeDuration;
        ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
        Material mat = psr != null ? psr.material : null;

        if (mat != null && mat.HasProperty("_Color"))
        {
            Color originalColor = mat.color;

            while (timer < totalDuration)
            {
                if (timer > fadeStart)
                {
                    float t = (timer - fadeStart) / fadeDuration;
                    Color c = originalColor;
                    c.a = Mathf.Lerp(originalColor.a, 0f, t);
                    mat.color = c;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }

        Destroy(ps.gameObject);
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
