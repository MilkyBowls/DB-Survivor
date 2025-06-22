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
    public float maxBeamDuration = 5f;

    private GameObject activeBeam;
    private BeamController beamController;

    private float growthTimer = 0f;
    private bool isFiring = false;

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
        if (beamController != null)
        {
            // Increase length while held
            growthTimer += Time.deltaTime;
            float dynamicLength = Mathf.Min(maxBeamLength, beamGrowthSpeed * growthTimer);
            beamController.SetBeamLength(dynamicLength);

            // Drain Ki
            float kiCost = kiDrainPerSecond * Time.deltaTime;
            if (player.currentKi >= kiCost)
            {
                player.currentKi -= kiCost;
            }
            else
            {
                EndBeam(); // stop beam if out of Ki
            }
        }
    }

    private void StartBeam()
    {
        if (!CanFire() || isFiring) return;

        isFiring = true;
        growthTimer = 0f;

        activeBeam = Instantiate(beamPrefab, firePoint.position, Quaternion.identity, transform);
        beamController = activeBeam.GetComponent<BeamController>();

        if (beamController != null)
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector2 world = Camera.main.ScreenToWorldPoint(mouseScreen);
            Vector2 aimDir = (world - (Vector2)firePoint.position).normalized;

            beamController.SetDirection(aimDir);
            beamController.SetBeamLength(0.1f); // initial small size
        }

        Invoke(nameof(EndBeam), maxBeamDuration); // auto-end after max duration
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
