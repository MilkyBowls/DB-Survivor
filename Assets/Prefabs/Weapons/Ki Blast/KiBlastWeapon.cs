using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class KiBlastWeapon : WeaponBase
{
    [Header("Ki Blast Settings")]
    public float blastSpeed = 10f;
    public float kiCostPerBlast = 5;

    [Header("Barrage Settings")]
    float homingDelay = 0.2f; // adjust to fit timing/spacing

    private bool isBarrageFiring = false;


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
        isBarrageFiring = true; // 🚫 Lock input

        int count = currentUpgrade?.barrageCount ?? 20;
        float delay = currentUpgrade?.barrageDelay ?? 0.05f;
        float arc = currentUpgrade?.visualConeAngle ?? 60f;
        float kiCost = currentUpgrade?.kiCost ?? kiCostPerBlast;

        for (int i = 0; i < count; i++)
        {
            if (!player.TrySpendKi(kiCost))
                break;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 aimDir = (mousePos - player.firePoint.position).normalized;
            float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

            float offset = Random.Range(-arc / 2f, arc / 2f);
            float finalAngle = aimAngle + offset;
            float rad = finalAngle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

            GameObject blast = FireBlastAndReturn(direction);
            if (blast != null)
            {
                KiBlast kiBlast = blast.GetComponent<KiBlast>();
                if (kiBlast != null)
                    kiBlast.homingDelay = homingDelay;
            }

            yield return new WaitForSeconds(delay);
        }

        float cooldownModifier = currentUpgrade?.fireCooldownModifier ?? 1f;
        float modifiedCooldown = fireCooldown * cooldownModifier;
        lastFireTime = Time.time + modifiedCooldown;

        isBarrageFiring = false; // ✅ Unlock input
    }






    private GameObject FireBlastAndReturn(Vector3 direction)
    {
        Vector3 firePointPos = player.firePoint.position;
        GameObject prefabToUse = currentUpgrade?.customProjectilePrefab ?? projectilePrefab;
        GameObject blast = Instantiate(prefabToUse, firePointPos, Quaternion.identity);



        // ? Apply scale cleanly
        float scale = currentUpgrade?.projectileScale ?? 1f;
        blast.transform.localScale = Vector3.one * scale;

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

        // ✅ Apply scale here too!
        float scale = currentUpgrade?.projectileScale ?? 1f;
        blast.transform.localScale = Vector3.one * scale;

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

        Debug.DrawRay(firePointPos, direction * 2f, Color.red, 1f);
    }

    protected override bool CanFire()
    {
        if (isBarrageFiring) return false; // 🔒 Lock out firing during active barrage

        float modifier = currentUpgrade?.fireCooldownModifier ?? 1f;
        float cooldown = fireCooldown * modifier;

        return Time.time >= lastFireTime + cooldown && player != null && !player.isDead;
    }


}
