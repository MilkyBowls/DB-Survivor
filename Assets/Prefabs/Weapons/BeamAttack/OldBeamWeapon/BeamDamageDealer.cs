using UnityEngine;

public class BeamDamageDealer : MonoBehaviour
{
    private BeamWeapon beamWeapon;
    private float damageTimer = 0f;
    private float damageInterval = 0.1f; // Optional: cooldown per target hit

    private void Awake()
    {
        // Search up the hierarchy for BeamWeapon once at startup
        beamWeapon = GetComponentInParent<BeamWeapon>();
        if (beamWeapon == null)
        {
            Debug.LogWarning("[BeamDamageDealer] Could not find BeamWeapon in parent hierarchy.");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (beamWeapon == null || !collision.CompareTag("Enemy")) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable == null) return;

        var upgradeData = beamWeapon.currentUpgrade;
        int baseDamage = beamWeapon.baseDamage;

        int finalDamage = Mathf.RoundToInt(baseDamage * (upgradeData?.damageModifier ?? 1f));

        bool isCrit = false;

        if (upgradeData?.enableCrits == true && Random.value <= 0.25f)
            isCrit = true;

        if (upgradeData?.guaranteedCritOnLowHealth == true &&
            damageable.CurrentHealth < damageable.MaxHealth * 0.5f)
            isCrit = true;

        if (isCrit)
            finalDamage *= 2;

        damageable.TakeDamage(finalDamage);

        if (upgradeData?.applyBurn == true)
            damageable.ApplyStatusEffect(StatusEffect.Burn, 3f);

        if (upgradeData?.applyStun == true)
            damageable.ApplyStatusEffect(StatusEffect.Stun, 1f);
    }
}
