using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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

    private List<GameObject> activeBeams = new List<GameObject>();
    private List<BeamController> beamControllers = new List<BeamController>();

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
        if (beamControllers.Count > 0 && isFiring)
        {
            player.PlayAttackAnimation();

            float scale = currentUpgrade?.projectileScale ?? 1f;
            float maxLength = maxBeamLength * (currentUpgrade?.beamMaxLengthModifier ?? 1f);
            float growthSpeed = beamGrowthSpeed * (currentUpgrade?.beamGrowthSpeedModifier ?? 1f);
            float initialLength = 6f * scale;

            growthTimer += Time.deltaTime;
            float dynamicLength = Mathf.Min(maxLength, initialLength + growthSpeed * growthTimer);

            float midScaleX = currentUpgrade?.beamVisualScaleX ?? 1f;
            float midScaleY = currentUpgrade?.beamVisualScaleY ?? 1f;

            int totalBeams = beamControllers.Count;
            float angleSpread = currentUpgrade?.beamSpreadAngle ?? 15f;

            for (int i = 0; i < totalBeams; i++)
            {
                float angleOffset = 0f;

                if (totalBeams > 1)
                {
                    float centerIndex = (totalBeams - 1) * 0.5f;
                    angleOffset = (i - centerIndex) * angleSpread;
                }

                Quaternion rotation = Quaternion.AngleAxis(angleOffset, Vector3.forward);
                Vector2 direction = rotation * firePoint.right;

                BeamController controller = beamControllers[i];
                controller.transform.position = firePoint.position;
                controller.transform.right = direction;
                controller.SetDirection(direction);
                controller.SetBeamWidth(beamWidth);
                controller.SetBeamVisuals(dynamicLength, scale, midScaleX, midScaleY);
            }

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

        int extraBeams = currentUpgrade?.extraBeams ?? 0;
        float angleSpread = currentUpgrade?.beamSpreadAngle ?? 15f;
        float totalBeams = 1 + extraBeams;

        float scale = currentUpgrade?.projectileScale ?? 1f;
        float midScaleX = currentUpgrade?.beamVisualScaleX ?? 1f;
        float midScaleY = currentUpgrade?.beamVisualScaleY ?? 1f;

        float maxLength = maxBeamLength * (currentUpgrade?.beamMaxLengthModifier ?? 1f);
        float growthSpeed = beamGrowthSpeed * (currentUpgrade?.beamGrowthSpeedModifier ?? 1f);
        float initialLength = 6f * scale;
        float dynamicLength = Mathf.Min(maxLength, initialLength + growthSpeed * growthTimer);

        for (int i = 0; i < totalBeams; i++)
        {
            float angleOffset = 0f;

            if (totalBeams > 1)
            {
                float centerIndex = (totalBeams - 1) * 0.5f;
                angleOffset = (i - centerIndex) * angleSpread;
            }

            Quaternion rotation = Quaternion.AngleAxis(angleOffset, Vector3.forward);
            Vector2 direction = rotation * firePoint.right;

            GameObject beam = Instantiate(beamPrefab, firePoint.position, Quaternion.identity, transform);
            BeamController controller = beam.GetComponent<BeamController>();

            if (controller != null)
            {
                controller.transform.right = direction;
                controller.SetDirection(direction);
                controller.SetBeamWidth(beamWidth);
                controller.SetBeamVisuals(dynamicLength, scale, midScaleX, midScaleY);
                beamControllers.Add(controller);
                activeBeams.Add(beam);
            }
        }

        float upgradedDuration = currentUpgrade?.beamDuration ?? baseBeamDuration;
        Invoke(nameof(EndBeam), upgradedDuration);
    }

    private void EndBeam()
    {
        if (!isFiring) return;

        isFiring = false;
        growthTimer = 0f;

        foreach (var beam in activeBeams)
        {
            if (beam != null)
                Destroy(beam);
        }

        activeBeams.Clear();
        beamControllers.Clear();
    }
}
