using System.Collections;
using UnityEngine;
using Pathfinding;

public class SaibamanEnemy : MonoBehaviour, IDamageable
{
    public EnemyStats stats;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => stats.maxHealth;

    private int currentHealth;
    public float explosionTriggerRadius = 1.5f;
    public float stickDistance = 0.2f;
    public float magnetSpeed = 5f;
    public float explosionDelay = 1f;
    private bool isExploding = false;

    private Animator animator;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Seeker seeker;

    private Path path;
    private int currentWaypoint = 0;
    public float nextWaypointDistance = 0.1f;
    public float repathRate = 1f;
    private bool reachedEndOfPath = false;

    public void ScaleHealth(int wave, float difficultyMultiplier)
    {
        int baseHealth = stats.maxHealth;
        int scaledHealth = Mathf.CeilToInt(baseHealth * (1 + difficultyMultiplier * wave));
        currentHealth = scaledHealth;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        currentHealth = stats.maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player != null)
            InvokeRepeating(nameof(UpdatePath), 0f, repathRate);
    }

    void UpdatePath()
    {
        if (!isExploding && player != null && seeker.IsDone())
            seeker.StartPath(rb.position, player.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (isExploding)
            return;

        if (path == null || player == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= explosionTriggerRadius && !isExploding)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), true);
            StartCoroutine(ExplodeSequence());
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }

        reachedEndOfPath = false;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        rb.MovePosition(rb.position + direction * stats.moveSpeed * Time.fixedDeltaTime);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (spriteRenderer != null)
            spriteRenderer.flipX = direction.x < 0;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (stats.deathEffect != null)
            Instantiate(stats.deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
        GetComponent<DropManager>()?.SpawnDrop();
    }

    private IEnumerator ExplodeSequence()
    {
        isExploding = true;

        PlayerController pc = player.GetComponent<PlayerController>();
        Transform target = pc != null && pc.enemyTargetPoint != null ? pc.enemyTargetPoint : player;

        spriteRenderer.flipX = target.position.x < transform.position.x;

        // Disable physics so we move manually
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        // 🌀 Smooth jump toward the player before attaching
        while (Vector2.Distance(transform.position, target.position) > stickDistance)
        {
            if (player == null || player.tag != "Player") yield break;

            Vector2 toTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(toTarget * magnetSpeed * Time.deltaTime);

            yield return null;
        }

        // ✅ Snap to latch point and stick
        if (pc != null && pc.saibamanLatchPoint != null)
        {
            transform.SetParent(pc.saibamanLatchPoint);
            transform.localPosition = Vector3.zero;
        }

        if (pc != null)
            pc.isLatched = true;

        if (animator != null)
            animator.SetTrigger("Explode");

        yield return new WaitForSeconds(explosionDelay);

        if (pc != null)
        {
            pc.TakeDamage(stats.damage);
            pc.isLatched = false;
        }

        transform.SetParent(null);
        Destroy(gameObject);
    }

    public void ApplyStatusEffect(StatusEffect effectType, float duration)
{
    switch (effectType)
    {
        case StatusEffect.Burn:
            StartCoroutine(ApplyBurn(duration));
            break;
        case StatusEffect.Stun:
            StartCoroutine(ApplyStun(duration));
            break;
    }
}

private IEnumerator ApplyBurn(float duration)
{
    float interval = 0.5f;
    int ticks = Mathf.FloorToInt(duration / interval);
    for (int i = 0; i < ticks; i++)
    {
        TakeDamage(1);
        yield return new WaitForSeconds(interval);
    }
}

private IEnumerator ApplyStun(float duration)
{
    // Temporarily stop movement/explosion AI
    bool wasExploding = isExploding;
    isExploding = true;
    yield return new WaitForSeconds(duration);
    isExploding = wasExploding;
}




    public void SaibamanExplosionShake() => Camera.main.GetComponent<CameraFollow>()?.SaibamanExplosion();

    public void PlaySaibamanExplosionSound() => SFXManager.Instance.Play(SFXManager.Instance.SaibamanExplosion);
}
