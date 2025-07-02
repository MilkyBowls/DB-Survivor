using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterTransformation
{
    public string formName;
    public Sprite transformationPortrait;
    public RuntimeAnimatorController animatorOverride;
    public AudioClip transformationSFX;

    [Header("Transformation Costs")]
    public float kiCostToTransform;

    [Header("Aura Overrides")]
    public AnimatorOverrideController auraAnimatorOverride;
    public List<ParticleSystem> auraChargeParticles;
    
    [Header("Aura Behavior")]
    public bool alwaysShowAuraParticles;

    [Header("Ki Behavior")]
    public float attackKiMultiplier;
    public float kiDrainPerSecond;

    [Header("Stat Multipliers")]
    public float healthMultiplier;
    public float damageMultiplier;
    public float speedMultiplier;
}
