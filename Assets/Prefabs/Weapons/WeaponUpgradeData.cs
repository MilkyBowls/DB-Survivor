using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Upgrade Tier")]
public class WeaponUpgradeData : ScriptableObject
{
    [Header("General Upgrade Info")]
    [Range(1, 10)] public int level;

    [Tooltip("Multiplier to apply to base damage.")]
    public float damageModifier = 1f;

    [Tooltip("Multiplier to apply to base projectile speed.")]
    public float projectileSpeedModifier = 1f;

    [Tooltip("Cooldown time multiplier (lower = faster).")]
    public float fireCooldownModifier = 1f;

    [Header("Projectile Behavior")]
    public bool enableDoubleShot = false;
    public bool enablePiercing = false;
    public bool enableExplosion = false;
    public bool enableCrits = false;
    public bool guaranteedCritOnLowHealth = false;

    [Header("Barrage Mode")]
    public bool enableBarrage = false;
    public int barrageCount = 0;
    public float barrageDelay = 0.05f;

    [Header("Advanced Barrage Effects")]
    public bool enableHoming = false;
    public bool applyBurn = false;
    public bool applyStun = false;
    public bool destroyEnemyProjectiles = false;
    public bool autoAimDuringBarrage = false;

    [Header("Visual & Gameplay Settings")]
    public float visualConeAngle = 60f;   // how wide the barrage cone is
    public float projectileScale = 1f;    // scale multiplier for visual size
    public float kiCost = 5;                // Ki cost per shot at this level

    [Header("Optional Custom Projectile")]
    public GameObject customProjectilePrefab;
}
