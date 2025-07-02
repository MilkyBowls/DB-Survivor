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
    private Vector3 currentScale;
    private Color baseColor;
    private bool isActive = false;
    private bool isCharging = false;
    private Coroutine particleLoopCoroutine;
    private List<ParticleSystem> persistentParticles = new List<ParticleSystem>();

    [Header("Charging Particle Effects")]
    public List<ParticleSystem> chargeParticlePrefabs;
    public Transform particleSpawnArea;
    public float particlesPerSecond = 4f;
    public Vector2 spawnRadius = new Vector2(0.5f, 0.5f);
    public float fadeDuration = 0.5f;

    [Header("Persistent Aura Particle Loop")]
    public bool usePersistentAuraLoop = true;
    public float persistentAuraInterval = 2f;
    public int persistentParticlesPerCycle = 2;
    private Coroutine persistentParticleCoroutine;


    void Start()
    {
        if (auraRenderer != null)
            baseColor = auraRenderer.color;

        defaultScale = transform.localScale;
        currentScale = defaultScale;
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
        }

        currentScale = defaultScale;
        transform.localScale = currentScale;
        isActive = true;
    }

    public void DisableAura()
    {
        if (auraAnimator != null)
        {
            auraAnimator.SetBool("IsActive", false);
            auraAnimator.SetBool("IsCharging", false);
            auraAnimator.Rebind();
            auraAnimator.Update(0f);
        }

        if (auraRenderer != null)
        {
            auraRenderer.enabled = false;
            auraRenderer.color = baseColor;
        }

        StopChargingParticles();

        isActive = false;
        isCharging = false;
        transform.localScale = defaultScale;
    }

    public void PlayChargeAnimation(bool charging)
    {
        if (!isActive || auraAnimator == null)
            return;

        Debug.Log($"[Aura] Charge Triggered: {charging}");

        if (charging)
        {
            if (isCharging) return;
            isCharging = true;

            auraAnimator.SetBool("IsCharging", true);
            auraAnimator.ResetTrigger("StartCharge");
            auraAnimator.SetTrigger("StartCharge");

            if (particleLoopCoroutine == null)
                particleLoopCoroutine = StartCoroutine(LoopChargeParticles());

            transform.localScale = currentScale;
        }
        else
        {
            if (!isCharging) return;
            isCharging = false;

            auraAnimator.SetBool("IsCharging", false);

            // Optional but effective: force immediate transition override
            auraAnimator.CrossFade("Idle", 0f); // 👈 this forces the Idle animation instantly

            StopChargingParticles();
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

        ParticleSystem ps = Instantiate(prefab, transform);
        ps.transform.localPosition = particleSpawnArea != null
            ? transform.InverseTransformPoint(particleSpawnArea.position)
            : Vector3.zero;

        ps.Play();
        StartCoroutine(FadeOutAndDestroy(ps, ps.main.duration + ps.main.startLifetime.constantMax));
    }


    private IEnumerator FadeOutAndDestroy(ParticleSystem ps, float totalDuration)
    {
        if (ps == null) yield break;

        float timer = 0f;
        float fadeStart = totalDuration - fadeDuration;

        ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
        Material mat = psr != null ? psr.material : null;
        Color originalColor = mat != null && mat.HasProperty("_Color") ? mat.color : Color.white;

        while (timer < totalDuration)
        {
            if (ps == null) yield break;

            if (mat != null && timer > fadeStart)
            {
                float t = (timer - fadeStart) / fadeDuration;
                Color c = originalColor;
                c.a = Mathf.Lerp(originalColor.a, 0f, t);
                mat.color = c;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (ps != null)
            Destroy(ps.gameObject);
    }

    public void ApplyTransformationAura(CharacterTransformation transformation)
    {
        ClearAuraState();

        foreach (var p in persistentParticles)
        {
            if (p != null)
                Destroy(p.gameObject);
        }
        persistentParticles.Clear();

        if (transformation == null) return;

        Debug.Log($"[Aura] Applying transformation: {transformation.formName}");
        Debug.Log($"[Aura] Animator override: {transformation.auraAnimatorOverride}");

        if (transformation.auraAnimatorOverride != null && auraAnimator != null)
        {
            auraAnimator.runtimeAnimatorController = transformation.auraAnimatorOverride;
        }

        if (transformation.auraChargeParticles != null && transformation.auraChargeParticles.Count > 0)
        {
            chargeParticlePrefabs = transformation.auraChargeParticles;

            if (transformation.alwaysShowAuraParticles)
            {
                int spawnCount = Mathf.Min(2, chargeParticlePrefabs.Count);

                List<int> usedIndices = new List<int>();
                for (int i = 0; i < spawnCount; i++)
                {
                    int index;
                    do
                    {
                        index = Random.Range(0, chargeParticlePrefabs.Count);
                    } while (usedIndices.Contains(index) && usedIndices.Count < chargeParticlePrefabs.Count);

                    usedIndices.Add(index);

                    ParticleSystem prefab = chargeParticlePrefabs[index];
                    ParticleSystem ps = Instantiate(prefab, transform);
                    ps.transform.localPosition = particleSpawnArea != null
                        ? transform.InverseTransformPoint(particleSpawnArea.position)
                        : Vector3.zero;

                    ps.Play();
                    persistentParticles.Add(ps);
                }

                // ✅ Moved outside the loop
                if (usePersistentAuraLoop && persistentParticleCoroutine == null)
                {
                    persistentParticleCoroutine = StartCoroutine(PersistentAuraLoop());
                }
            }

        }

    }

    public void ClearAuraState()
    {
        if (auraAnimator != null)
        {
            auraAnimator.runtimeAnimatorController = null;
            auraAnimator.Rebind();
            auraAnimator.Update(0f);
        }

        if (particleLoopCoroutine != null)
        {
            StopCoroutine(particleLoopCoroutine);
            particleLoopCoroutine = null;
        }

        foreach (var p in persistentParticles)
        {
            if (p != null)
                Destroy(p.gameObject);
        }
        persistentParticles.Clear();

        foreach (Transform child in transform)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(ps.gameObject);
        }

        chargeParticlePrefabs = new List<ParticleSystem>();
        isCharging = false;

        if (persistentParticleCoroutine != null)
        {
            StopCoroutine(persistentParticleCoroutine);
            persistentParticleCoroutine = null;
        }
    }

    private void StopChargingParticles()
    {
        if (particleLoopCoroutine != null)
        {
            StopCoroutine(particleLoopCoroutine);
            particleLoopCoroutine = null;
        }

        foreach (Transform child in transform)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            if (ps != null && !persistentParticles.Contains(ps))
            {
                ps.Stop();
                Destroy(ps.gameObject, fadeDuration);
            }
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
    private IEnumerator PersistentAuraLoop()
    {
        while (true)
        {
            int spawnCount = Mathf.Min(persistentParticlesPerCycle, chargeParticlePrefabs.Count);
            HashSet<int> usedIndices = new HashSet<int>();

            for (int i = 0; i < spawnCount; i++)
            {
                int index;
                do
                {
                    index = Random.Range(0, chargeParticlePrefabs.Count);
                }
                while (usedIndices.Contains(index) && usedIndices.Count < chargeParticlePrefabs.Count);

                usedIndices.Add(index);

                ParticleSystem prefab = chargeParticlePrefabs[index];
                ParticleSystem ps = Instantiate(prefab, transform);

                ps.transform.localPosition = particleSpawnArea != null
                    ? transform.InverseTransformPoint(particleSpawnArea.position)
                    : Vector3.zero;

                ps.Play();
                persistentParticles.Add(ps);

                // Let it destroy itself naturally after playing
                float lifetime = ps.main.duration + ps.main.startLifetime.constantMax;
                Destroy(ps.gameObject, lifetime);
            }

            yield return new WaitForSeconds(persistentAuraInterval);
        }
    }


}
