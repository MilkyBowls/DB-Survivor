using UnityEngine;

public class KiBlast : MonoBehaviour
{
    [Header("Lifetime & Damage")]
    public float lifetime = 3f;
    public int baseDamage = 1;

    [Header("Impact FX")]
    public GameObject impactFXPrefab;

    [Header("Upgrade Behavior")]
    public WeaponUpgradeData upgradeData;

    private int enemiesPierced = 0;
    private Rigidbody2D rb;
    private Transform targetEnemy;

    [Header("Homing Settings")]
    public float homingDelay = 0.5f; // time in seconds before homing activates

    private bool homingActive = false;
    private float homingStartTime;

    [Header("Advanced Homing")]
    public float homingActivationRadius = 3f; // how close to activate homing
    public float homingTurnSpeed = 5f;        // how fast it can steer (degrees per second)


    [Header("Visual Spawn FX")]
    public float fadeInDuration = 0.2f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float scalePunchAmount = 1.2f; // how big it grows initially
    public float scaleSettleSpeed = 5f;   // how fast it returns to normal

    private SpriteRenderer spriteRenderer;
    private float fadeStartTime;
    private bool isFadingIn = true;
    private Vector3 originalScale;

    private Vector3 receivedInitialScale;

    private CircleCollider2D circleCollider;
    private float originalColliderRadius;

    [HideInInspector]
    public float projectileScale = 1f;


    void Start()
{
    Destroy(gameObject, lifetime);
    rb = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();

    if (spriteRenderer != null)
    {
        Color c = spriteRenderer.color;
        c.a = 0f;
        spriteRenderer.color = c;
    }

    receivedInitialScale = transform.localScale;
    transform.localScale = receivedInitialScale * scalePunchAmount;

    originalScale = receivedInitialScale;

    fadeStartTime = Time.time;
    isFadingIn = true;

    if (upgradeData != null && upgradeData.enableHoming)
    {
        FindInitialTarget();
        homingStartTime = Time.time + homingDelay;
    }

    circleCollider = GetComponent<CircleCollider2D>();
    if (circleCollider != null)
    {
        originalColliderRadius = circleCollider.radius;
        // Scale radius based on visual scale
        float scaleFactor = transform.localScale.x;
        circleCollider.radius = originalColliderRadius * scaleFactor;
    }
    }

    void Update()
    {
        if (isFadingIn && spriteRenderer != null)
        {
            float t = (Time.time - fadeStartTime) / fadeInDuration;
            float easedT = fadeInCurve.Evaluate(Mathf.Clamp01(t));

            // Fade in
            Color c = spriteRenderer.color;
            c.a = easedT;
            spriteRenderer.color = c;

            // Scale down from punch to normal
            transform.localScale = Vector3.Lerp(receivedInitialScale * scalePunchAmount, receivedInitialScale, easedT);


            if (t >= 1f)
                isFadingIn = false;
        }

        if (upgradeData?.enableHoming == true && targetEnemy != null)
        {
            if (!homingActive && Time.time >= homingStartTime)
                homingActive = true;

            if (homingActive)
            {
                float distToTarget = Vector2.Distance(transform.position, targetEnemy.position);

                if (distToTarget <= homingActivationRadius)
                {
                    Vector2 toTarget = ((Vector2)targetEnemy.position - rb.position).normalized;
                    Vector2 currentDir = rb.linearVelocity.normalized;
                    float speed = rb.linearVelocity.magnitude;

                    // Smoothly rotate current velocity toward target direction
                    float angle = Vector2.SignedAngle(currentDir, toTarget);
                    float maxAngle = homingTurnSpeed * Time.deltaTime;
                    float clampedAngle = Mathf.Clamp(angle, -maxAngle, maxAngle);

                    Vector2 newDir = Quaternion.Euler(0, 0, clampedAngle) * currentDir;
                    rb.linearVelocity = newDir.normalized * speed;

                    float visualAngle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, visualAngle);
                }
            }
        }
    }



    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            bool shouldDestroy = false;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                int finalDamage = Mathf.RoundToInt(baseDamage * (upgradeData?.damageModifier ?? 1f));

                bool isCrit = false;

                if (upgradeData?.enableCrits == true && Random.value <= 0.25f)
                    isCrit = true;

                if (upgradeData?.guaranteedCritOnLowHealth == true && damageable.CurrentHealth < damageable.MaxHealth * 0.5f)
                    isCrit = true;

                if (isCrit)
                    finalDamage *= 2;

                damageable.TakeDamage(finalDamage);

                if (upgradeData?.applyBurn == true)
                    damageable.ApplyStatusEffect(StatusEffect.Burn, 3f);

                if (upgradeData?.applyStun == true)
                    damageable.ApplyStatusEffect(StatusEffect.Stun, 1f);

                // Always show impact FX on hit
                TriggerImpactFX();

                // Explosion
                if (upgradeData?.enableExplosion == true)
                    ExplosionDamage(transform.position, 2f);

                enemiesPierced++;

                if (upgradeData == null || !upgradeData.enablePiercing || enemiesPierced > 2)
                    shouldDestroy = true;
            }

            if (shouldDestroy)
            {
                Destroy(gameObject);
            }
        }
        else if (collision.CompareTag("Prop"))
        {
            TriggerImpactFX();
            Destroy(gameObject);
        }
        else if (upgradeData?.destroyEnemyProjectiles == true && collision.CompareTag("EnemyProjectile"))
        {
            Destroy(collision.gameObject);
        }
    }


    private void TriggerImpactFX()
    {
        if (impactFXPrefab != null)
        {
            GameObject impact = Instantiate(impactFXPrefab, transform.position, Quaternion.identity);

            // Rotation variation
            impact.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            // Scale variation
            float baseScale = (upgradeData != null ? upgradeData.projectileScale : 1f) * 0.6f;
            float variedScale = baseScale * Random.Range(0.9f, 1.1f);
            impact.transform.localScale = Vector3.one * variedScale;

            // Optional: Color variation if SpriteRenderer exists
            SpriteRenderer sr = impact.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = sr.color;
                color.r *= Random.Range(0.9f, 1.1f);
                color.g *= Random.Range(0.9f, 1.1f);
                color.b *= Random.Range(0.9f, 1.1f);
                sr.color = color;
            }
        }
    }


    private void ExplosionDamage(Vector3 center, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    int aoeDamage = Mathf.RoundToInt(baseDamage * 0.5f);
                    damageable.TakeDamage(aoeDamage);
                }
            }
        }
    }

    private void FindInitialTarget()
    {
        float homingRadius = 8f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, homingRadius);

        float closestDistance = float.MaxValue;
        Transform closestEnemy = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy;
        }
    }
}
