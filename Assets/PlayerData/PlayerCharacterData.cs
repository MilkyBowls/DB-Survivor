using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DBZ/Player Character Data")]
public class PlayerCharacterData : ScriptableObject
{
    [Header("Identity")]
    public string characterName;
    public Sprite portrait;
    public RuntimeAnimatorController animatorController;

    [Header("Audio")]
    public AudioClip[] voiceLines;
    public AudioClip transformationShout;

    [Header("Starting Weapon")]
    public GameObject startingWeaponPrefab;

    [Header("Base Aura Profile")]
    public CharacterTransformation baseAuraProfile;

    [Header("Base Stats")]
    public float maxHealth;
    public float kiDamage;
    public float meleeDamage;
    public float movementSpeed;
    public float dodgeChance;
    public float kiRegenRate;
    public float attackSpeed;
    public float critChance;
    public float maxKiCapacity;
    public float defense;
    public float luck;
    public float abilityCooldown;
    public float stamina;

    [Header("Transformations")]
    public List<CharacterTransformation> transformations;

    [Header("Passive Abilities")]
    public List<PassiveAbility> passiveAbilities;
}
