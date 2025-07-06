using UnityEngine;
using Pathfinding;
using System.Collections;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public EnemyStats stats;
    private Transform player;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Seeker seeker;

    private Path path;
    private int currentWaypoint = 0;
    public float nextWaypointDistance = 0.1f;
    private bool reachedEndOfPath = false;
    public float repathRate = 1f;

    private bool isStunned = false;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => stats.maxHealth;

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
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = stats.maxHealth;

        if (player != null)
            InvokeRepeating(nameof(UpdatePath), 0f, repathRate);
    }

    void UpdatePath()
    {
        if (!isStunned && player != null && seeker.IsDone())
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
        if (isStunned || path == null || player == null)
            return;

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

    public void TakeDamage(int amount, float delayBeforeDeath)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            StartCoroutine(DelayedDeath(delayBeforeDeath));
        }
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, 0.2f); // Default fallback
    }

    private IEnumerator DelayedDeath(float delay)
    {
        yield return new WaitForSeconds(delay);
        Die();
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
            TakeDamage(1); // Burn damage per tick
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator ApplyStun(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    private void Die()
    {
        if (stats.deathEffect != null)
            Instantiate(stats.deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(stats.damage);
            }
        }
    }
}
