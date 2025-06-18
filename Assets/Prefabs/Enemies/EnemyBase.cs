using UnityEngine;
using Pathfinding;
using System.Collections;

public class EnemyBase : MonoBehaviour
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

        if (player != null)
            InvokeRepeating(nameof(UpdatePath), 0f, repathRate);
    }

    void UpdatePath()
    {
        if (player != null && seeker.IsDone())
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
        if (path == null || player == null)
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
