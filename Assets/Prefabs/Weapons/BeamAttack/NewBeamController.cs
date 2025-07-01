using System.Collections;
using UnityEngine;

public class NewBeamController : MonoBehaviour
{
    public Animator animator;
    public Collider2D damageCollider;

    private WeaponUpgradeData upgradeData;
    private int baseDamage = 1;
    private bool damageEnabled = false;

    public void Initialize(WeaponUpgradeData upgrade, float duration, int baseDamage)
    {
        Debug.Log($"[NewBeamController] Initialized with damage {baseDamage}, duration {duration}");
        this.upgradeData = upgrade;
        this.baseDamage = baseDamage;
        StartCoroutine(BeamRoutine(duration));
    }

    //     private void Update()
    // {
    //     if (followTarget != null)
    //     {
    //         transform.position = followTarget.position;

    //         float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
    //         transform.rotation = Quaternion.Euler(0f, 0f, angle);
    //     }
    // }

    private IEnumerator BeamRoutine(float duration)
    {
        // Wait for BeamStart animation to finish before enabling damage
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);

        damageEnabled = true;
        damageCollider.enabled = true;

        yield return new WaitForSeconds(duration);

        damageEnabled = false;
        damageCollider.enabled = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!damageEnabled || !other.CompareTag("Enemy")) return;

        var target = other.GetComponent<IDamageable>();
        if (target == null) return;

        int finalDamage = Mathf.RoundToInt(baseDamage * (upgradeData?.damageModifier ?? 1f));
        bool isCrit = false;

        if (upgradeData?.enableCrits == true && Random.value <= 0.25f)
            isCrit = true;

        if (upgradeData?.guaranteedCritOnLowHealth == true &&
            target.CurrentHealth < target.MaxHealth * 0.5f)
            isCrit = true;

        if (isCrit)
            finalDamage *= 2;

        target.TakeDamage(finalDamage);

        if (upgradeData?.applyBurn == true)
            target.ApplyStatusEffect(StatusEffect.Burn, 3f);

        if (upgradeData?.applyStun == true)
            target.ApplyStatusEffect(StatusEffect.Stun, 1f);
    }

    private Transform followTarget;
    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    private Vector2 fireDirection;
        public void SetDirection(Vector2 direction)
        {
            fireDirection = direction.normalized;
            float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0f, 0f, angle); // ← localRotation since it's parented!
        }
}
