using UnityEngine;

public class PropDestructible : MonoBehaviour
{
    public bool destroyOnHit = false;
    public GameObject destroyEffect;

    public void TakeDamage(int amount)
    {
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
