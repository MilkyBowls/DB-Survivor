using UnityEngine;

[System.Serializable]
public class CharacterTransformation
{
    public string formName;
    public Sprite transformationPortrait;
    public RuntimeAnimatorController animatorOverride;
    public Sprite auraSprite;
    public AudioClip transformationSFX;

    [Header("Transformation Costs")]
    public float kiCostToTransform;

    [Header("Ki Behavior")]
    public float attackKiMultiplier;
    public float kiDrainPerSecond;

    [Header("Stat Multipliers")]
    public float healthMultiplier;
    public float damageMultiplier;
    public float speedMultiplier;
}
