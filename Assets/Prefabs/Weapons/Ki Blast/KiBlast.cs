using UnityEngine;

public class KiBlast : MonoBehaviour
{
    public float lifetime = 3f;
    public int damage = 1;
    public GameObject impactFXPrefab;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Trigger hit: {collision.name}");

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy and applying damage");

            SFXManager.Instance.Play(SFXManager.Instance.kiBlastImpact);

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            if (impactFXPrefab != null)
            {
                Instantiate(impactFXPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }

        // Hit Prop
        if (collision.CompareTag("Prop"))
        {
            SFXManager.Instance.Play(SFXManager.Instance.kiBlastImpact);

            PropDestructible prop = collision.GetComponent<PropDestructible>();
            if (prop != null)
            {
                if (impactFXPrefab != null)
                {
                    Instantiate(impactFXPrefab, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }



    void TriggerImpactFX()
    {
        if (impactFXPrefab != null)
        {
            Instantiate(impactFXPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
