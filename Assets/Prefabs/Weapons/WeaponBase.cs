using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject projectilePrefab;
    public float fireCooldown = 0.5f;

    protected float lastFireTime = -999f;
    protected PlayerController player;
    private PlayerControls controls;
    private bool isFiring = false;

    protected virtual void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
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
        return Time.time >= lastFireTime + fireCooldown && player != null && !player.isDead;
    }

    protected virtual void Fire()
    {
        if (!player.TrySpendKi(10)) return;

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
}
