using UnityEngine;
using UnityEngine.InputSystem;

public class BeamWeapon : WeaponBase
{
    [Header("Beam Prefab")]
    public GameObject beamPrefab;
    private Transform firePoint;

    [Header("Beam Behavior")]
    public float beamGrowthSpeed = 4f;
    public float maxBeamLength = 15f;
    public float beamWidth = 1f;
    public float kiDrainPerSecond = 10f;

    private GameObject activeBeam;
    private BeamController beamController;

    private float growthTimer = 0f;

    [Tooltip("Default beam duration if upgrade data is missing.")]
    public float baseBeamDuration = 5f;

    [Tooltip("Explosion prefab or visual effect to trigger when beam ends.")]
    public GameObject explosionPrefab;

    [SerializeField] private float visualScaleX = 1f;
    [SerializeField] private float visualScaleY = 1f;

    [HideInInspector] public int baseDamage = 1;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<PlayerController>();
        firePoint = player != null ? player.firePoint : null;

        if (firePoint == null)
            Debug.LogWarning("BeamWeapon: firePoint is null!");
    }

    private void OnEnable()
    {
        controls = new PlayerControls();
        controls.Player.Enable();
        controls.Player.Weapon.started += ctx => StartBeam();
        controls.Player.Weapon.canceled += ctx => EndBeam();
    }

    private void OnDisable()
    {
        controls?.Player.Disable();
    }

    private void Update()
    {
        if (beamController != null && isFiring)
        {
            player.PlayAttackAnimation();

            activeBeam.transform.position = firePoint.position;

            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector2 world = Camera.main.ScreenToWorldPoint(mouseScreen);
            Vector2 aimDir = (world - (Vector2)firePoint.position).normalized;
            beamController.SetDirection(aimDir);

            float scale = currentUpgrade?.projectileScale ?? 1f;
            float maxLength = maxBeamLength * (currentUpgrade?.beamMaxLengthModifier ?? 1f);
            float growthSpeed = beamGrowthSpeed * (currentUpgrade?.beamGrowthSpeedModifier ?? 1f);
            float initialLength = 6f * scale;

            growthTimer += Time.deltaTime;
            float dynamicLength = Mathf.Min(maxLength, initialLength + growthSpeed * growthTimer);

            float midScaleX = currentUpgrade?.beamVisualScaleX ?? 1f;
            float midScaleY = currentUpgrade?.beamVisualScaleY ?? 1f;

            beamController.SetBeamWidth(beamWidth);
            beamController.SetBeamVisuals(dynamicLength, scale, midScaleX, midScaleY);

            float kiCost = kiDrainPerSecond * Time.deltaTime;
            if (player.currentKi >= kiCost)
            {
                player.currentKi -= kiCost;
            }
        }
    }

    private void StartBeam()
    {
        if (!CanFire() || isFiring) return;

        CancelInvoke(nameof(EndBeam));
        isFiring = true;
        growthTimer = 0f;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 world = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 aimDir = (world - (Vector2)firePoint.position).normalized;

        activeBeam = Instantiate(beamPrefab, firePoint.position, Quaternion.identity, transform);
        beamController = activeBeam.GetComponent<BeamController>();

        if (beamController != null)
        {
            beamController.SetDirection(aimDir);

            float scale = currentUpgrade?.projectileScale ?? 1f;
            float midScaleX = currentUpgrade?.beamVisualScaleX ?? 1f;
            float midScaleY = currentUpgrade?.beamVisualScaleY ?? 1f;

            beamController.SetBeamWidth(beamWidth);
            beamController.SetBeamVisuals(6f * scale, scale, midScaleX, midScaleY);
        }

        float upgradedDuration = currentUpgrade?.beamDuration ?? baseBeamDuration;
        Invoke(nameof(EndBeam), upgradedDuration);
    }

    private void EndBeam()
    {
        if (!isFiring) return;

        isFiring = false;
        growthTimer = 0f;

        if (activeBeam != null)
            Destroy(activeBeam);

        beamController = null;
        activeBeam = null;
    }
}