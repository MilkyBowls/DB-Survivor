using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class NewBeamWeapon : WeaponBase
{
    [Header("Beam Prefab")]
    public GameObject beamPrefab;

    [Header("Beam Behavior")]
    public float beamDuration = 2f;
    public float kiDrainPerSecond = 10f;

    private Transform firePoint;
    private FirePointController firePointController;

    private readonly List<GameObject> activeBeams = new();

    [Header("Base Stats")]
    public int baseDamage = 1;

    protected override void Awake()
    {
        base.Awake();
        firePoint = player?.firePoint;

        firePointController = player?.firePoint?.GetComponent<FirePointController>();

        if (firePoint == null)
            Debug.LogWarning("[NewBeamWeapon] firePoint is null!");
    }

    private void OnEnable()
    {
        controls = new PlayerControls();
        controls.Player.Enable();

        controls.Player.Weapon.started += ctx =>
        {
            Debug.Log("[Input] Weapon.started fired");
            StartBeam();
        };

        controls.Player.Weapon.canceled += ctx =>
        {
            Debug.Log("[Input] Weapon.canceled fired");
            EndBeam();
        };
    }

    private void OnDisable()
    {
        controls?.Player.Disable();
    }

    private void Update()
    {
        if (!isFiring) return;

        player.PlayAttackAnimation();

        float kiCost = kiDrainPerSecond * Time.deltaTime;
        if (player.currentKi >= kiCost)
        {
            player.currentKi -= kiCost;
        }
        else
        {
            EndBeam();
        }
    }

    private void StartBeam()
    {
        if (!CanFire() || isFiring) return;

        if (firePointController != null && firePointController.IsAimingTooClose)
        {
            Debug.Log("[NewBeamWeapon] Aiming too close — beam will not fire.");
            return;
        }

        isFiring = true;

        int extraBeams = currentUpgrade?.extraBeams ?? 0;
        int totalBeams = 1 + extraBeams;
        float angleSpread = currentUpgrade?.beamSpreadAngle ?? 15f;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 baseDirection = (mouseWorldPos - firePoint.position).normalized;

        for (int i = 0; i < totalBeams; i++)
        {
            float angleOffset = 0f;
            if (totalBeams > 1)
            {
                float centerIndex = (totalBeams - 1) * 0.5f;
                angleOffset = (i - centerIndex) * angleSpread;
            }

            Quaternion spreadRotation = Quaternion.AngleAxis(angleOffset, Vector3.forward);
            Vector2 finalDirection = spreadRotation * baseDirection;
            float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg;
            Quaternion beamRotation = Quaternion.Euler(0f, 0f, angle);

            GameObject beam = Instantiate(beamPrefab, firePoint.position, beamRotation, firePoint); // 🔁 parented to firePoint
            var beamComp = beam.GetComponent<NewBeamController>();

            float scale = currentUpgrade?.projectileScale ?? 1f;
            beam.transform.localScale = Vector3.one * scale;

            if (beamComp != null)
            {
                int finalDamage = Mathf.RoundToInt(baseDamage * (currentUpgrade?.damageModifier ?? 1f));
                beamComp.Initialize(currentUpgrade, beamDuration, finalDamage);
            }

            activeBeams.Add(beam);
        }

        Invoke(nameof(EndBeam), beamDuration);
    }


    private void EndBeam()
    {
        if (!isFiring) return;

        isFiring = false;

        foreach (var beam in activeBeams)
        {
            if (beam != null)
                Destroy(beam);
        }

        activeBeams.Clear();
    }
}
