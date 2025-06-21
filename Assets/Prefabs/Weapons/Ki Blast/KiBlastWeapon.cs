using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class KiBlastWeapon : WeaponBase
{
    [Header("Ki Blast Settings")]
    public float blastSpeed = 10f;
    public int kiCostPerBlast = 5;

    [Header("Barrage Spread")]
    public float fanArcAngle = 60f; // total arc range in degrees
    float homingDelay = 0.2f; // adjust to fit timing/spacing


    protected override void Fire()
    {
        if (currentUpgrade != null && currentUpgrade.enableBarrage)
        {
            if (!player.TrySpendKi(kiCostPerBlast)) return;
            StartCoroutine(FireBarrage());
            return;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        Vector3 direction = (mouseWorldPos - player.transform.position).normalized;

        if (!player.TrySpendKi(kiCostPerBlast)) return;
        player.PlayAttackAnimation();

        // Fire single or double shot depending on upgrade
        if (currentUpgrade != null && currentUpgrade.enableDoubleShot)
        {
            FireBlast(direction);
            FireBlast(Quaternion.Euler(0, 0, 10) * direction); // slight angle difference
        }
        else
        {
            FireBlast(direction);
        }

        lastFireTime = Time.time;
    }

private IEnumerator FireBarrage()
{
    int count = currentUpgrade?.barrageCount ?? 20;
    float delay = currentUpgrade?.barrageDelay ?? 0.05f;
    float arc = fanArcAngle; // full cone width
    float homingDelay = 0.2f;

    for (int i = 0; i < count; i++)
    {
        // Get current aim direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 aimDir = (mousePos - player.firePoint.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        // Pick a random angle within the cone
        float randomOffset = Random.Range(-arc / 2f, arc / 2f);
        float finalAngle = aimAngle + randomOffset;
        float rad = finalAngle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

        // Fire blast and delay homing
        GameObject blast = FireBlastAndReturn(direction);
        if (blast != null)
        {
            KiBlast kiBlast = blast.GetComponent<KiBlast>();
            if (kiBlast != null)
                kiBlast.homingDelay = homingDelay;
        }

        // Optional: draw cone direction
        Debug.DrawRay(player.firePoint.position, direction * 2f, Color.yellow, 0.75f);

        yield return new WaitForSeconds(delay);
    }

    lastFireTime = Time.time;
}



private GameObject FireBlastAndReturn(Vector3 direction)
{
    Vector3 firePointPos = player.firePoint.position;
    GameObject prefabToUse = currentUpgrade?.customProjectilePrefab ?? projectilePrefab;
    GameObject blast = Instantiate(prefabToUse, firePointPos, Quaternion.identity);

    SFXManager.Instance.Play(SFXManager.Instance.kiBlast);

    Rigidbody2D rb = blast.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        float speed = blastSpeed * (currentUpgrade?.projectileSpeedModifier ?? 1f);
        rb.linearVelocity = direction.normalized * speed;
    }

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    blast.transform.rotation = Quaternion.Euler(0, 0, angle);

    KiBlast kiBlast = blast.GetComponent<KiBlast>();
    if (kiBlast != null)
    {
        kiBlast.baseDamage = Mathf.RoundToInt(kiBlast.baseDamage * (currentUpgrade?.damageModifier ?? 1f));
        kiBlast.upgradeData = currentUpgrade;
    }

    return blast;
}




private void FireBlast(Vector3 direction)
{
    Vector3 firePointPos = player.firePoint.position;
    GameObject prefabToUse = currentUpgrade?.customProjectilePrefab ?? projectilePrefab;
    GameObject blast = Instantiate(prefabToUse, firePointPos, Quaternion.identity);

    SFXManager.Instance.Play(SFXManager.Instance.kiBlast);

    Rigidbody2D rb = blast.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        float speed = blastSpeed * (currentUpgrade?.projectileSpeedModifier ?? 1f);
        rb.linearVelocity = direction.normalized * speed;
    }

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    blast.transform.rotation = Quaternion.Euler(0, 0, angle);

    KiBlast kiBlast = blast.GetComponent<KiBlast>();
    if (kiBlast != null)
    {
        kiBlast.baseDamage = Mathf.RoundToInt(kiBlast.baseDamage * (currentUpgrade?.damageModifier ?? 1f));
        kiBlast.upgradeData = currentUpgrade;
    }

    // Optional: debug ray to visualize spread
    Debug.DrawRay(firePointPos, direction * 2f, Color.red, 1f);
}

}
