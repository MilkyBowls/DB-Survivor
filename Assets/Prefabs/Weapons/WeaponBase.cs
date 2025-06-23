using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject projectilePrefab;
    public float fireCooldown = 0.5f;

    protected float lastFireTime = -999f;
    protected PlayerController player;
    protected PlayerControls controls;
    protected bool isFiring = false;

    public int weaponLevel = 1;
    public WeaponUpgradeData[] upgrades;
    protected WeaponUpgradeData currentUpgrade;

    protected virtual void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        controls = new PlayerControls();
        ApplyUpgrade();
    }

    protected virtual void ApplyUpgrade()
    {
        currentUpgrade = System.Array.Find(upgrades, u => u.level == weaponLevel);
        if (currentUpgrade == null)
            Debug.LogWarning($"[BeamWeapon] No upgrade data found for weapon level {weaponLevel}");
        else
            Debug.Log($"[BeamWeapon] Upgrade applied: level {currentUpgrade.level}");
    }

    private void OnEnable()
    {
        if (controls == null)
            controls = new PlayerControls(); // 🛠️ This fixes the crash!

        controls.Player.Enable();
        controls.Player.Weapon.performed += OnFirePerformed;
        controls.Player.Weapon.canceled += OnFireCanceled;
    }

    private void OnDisable()
    {
        controls.Player.Weapon.performed -= OnFirePerformed;
        controls.Player.Weapon.canceled -= OnFireCanceled;
        controls.Player.Disable();
    }

    private void OnFireCanceled(InputAction.CallbackContext ctx)
    {
        isFiring = false;
    }

    private void OnFirePerformed(InputAction.CallbackContext ctx)
    {
        if (CanFire())
            Fire();
    }

    protected virtual bool CanFire()
    {
        float modifier = currentUpgrade != null ? currentUpgrade.fireCooldownModifier : 1f;
        float cooldown = fireCooldown * modifier;

        return Time.time >= lastFireTime + cooldown && player != null && !player.isDead;
    }

    protected virtual void Fire()
    {
        if (!player.TrySpendKi(1)) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        Vector3 direction = (mouseWorldPos - player.transform.position).normalized;
        Vector3 firePointPos = player.firePoint != null ? player.firePoint.position : player.transform.position;

        GetComponentInParent<PlayerController>()?.PlayAttackAnimation();

        GameObject projectile = Instantiate(projectilePrefab, firePointPos, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * 10f;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        lastFireTime = Time.time;
    }

    public void LevelUpWeapon()
    {
        weaponLevel = Mathf.Clamp(weaponLevel + 1, 1, 10);
        ApplyUpgrade();
    }

}
