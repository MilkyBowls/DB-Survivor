using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Upgrade Tier")]
public class WeaponUpgradeData : ScriptableObject
{
    [Header("General Upgrade Info")]
    [Range(1, 10)] public int level = 1;

    [Tooltip("Multiplier to apply to base damage.")]
    public float damageModifier = 1f;

    [Tooltip("Multiplier to apply to base projectile speed.")]
    public float projectileSpeedModifier = 1f;

    [Tooltip("Cooldown time multiplier (lower = faster).")]
    public float fireCooldownModifier = 1f;

    public float projectileScale = 1f;

    [Space(10)]
    [Header("Ki Blast - Base Effects")]
    public bool enableDoubleShot = false;
    public bool enablePiercing = false;
    public bool enableExplosion = false;
    public bool enableCrits = false;
    public bool guaranteedCritOnLowHealth = false;

    [Space(10)]
    [Header("Ki Blast - Barrage Mode")]
    public bool enableBarrage = false;
    public int barrageCount = 0;
    public float barrageDelay = 0.05f;

    [Space(10)]
    [Header("Ki Blast - Advanced Effects")]
    public bool enableHoming = false;
    public bool applyBurn = false;
    public bool applyStun = false;
    public bool destroyEnemyProjectiles = false;
    public bool autoAimDuringBarrage = false;

    [Space(10)]
    [Header("Ki Blast - Visuals & Gameplay")]
    public float visualConeAngle = 60f;
    public float kiCost = 5f;

    [Tooltip("Optional override projectile prefab.")]
    public GameObject customProjectilePrefab;

    [Space(15)]
    [Header("Beam Weapon - General Settings")]

    [Tooltip("Multiplier applied to max beam length.")]
    public float beamMaxLengthModifier = 1f;

    [Tooltip("Multiplier applied to beam growth speed.")]
    public float beamGrowthSpeedModifier = 1f;

    [Tooltip("Max beam duration in seconds at this upgrade level.")]
    public float beamDuration = 5f;

    public bool beamFollowsMouse = false;

    [Header("Beam Weapon - Visual Scale")]
    public float beamVisualScaleX = 1f;
    public float beamVisualScaleY = 1f;

    [Header("Beam Weapon - Branching")]
    [Tooltip("Number of extra beams to spawn (in addition to the main one).")]
    public int extraBeams = 0;

    [Tooltip("Degrees between beams (e.g., 15 = ±15° around the center beam).")]
    public float beamSpreadAngle = 15f;

    [Header("Beam Weapon - Visual Overrides")]
    public AnimatorOverrideController beamStartOverride;
    public AnimatorOverrideController beamEndOverride;
    public Sprite beamMidOverrideSprite;

    [Tooltip("Base pixel height of the beam mid sprite before visual scaling.")]
    public float beamMidBaseHeight = 4f;

    [Header("Beam Weapon - Animated Mid Sprite")]
    public List<Sprite> animatedMidFrames;
    public float animatedMidFrameRate = 12f;
}

