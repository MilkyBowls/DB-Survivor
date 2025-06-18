using UnityEngine;
using UnityEngine.InputSystem;

public class KiBlastWeapon : WeaponBase
{
    public float blastSpeed = 10f;
    public int kiCostPerBlast = 5;

    protected override void Fire()
    {
        if (!player.TrySpendKi(kiCostPerBlast)) return;

        player.PlayAttackAnimation(); // <-- This line was missing

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        Vector3 direction = (mouseWorldPos - player.transform.position).normalized;
        Vector3 firePointPos = player.firePoint.position;

        GameObject blast = Instantiate(projectilePrefab, firePointPos, Quaternion.identity);

        SFXManager.Instance.Play(SFXManager.Instance.kiBlast);

        Rigidbody2D rb = blast.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * blastSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        blast.transform.rotation = Quaternion.Euler(0, 0, angle);

        lastFireTime = Time.time;
    }

}
