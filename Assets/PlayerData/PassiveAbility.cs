using UnityEngine;

[System.Serializable]
public class PassiveAbility
{
    public string abilityName;
    [TextArea] public string description;

    public enum PassiveType
    {
        StatBoost,
        OnKillEffect,
        OnHitEffect,
        OnLowHealth,
        Regeneration,
        Shield,
        Custom
    }

    public PassiveType type;

    [Header("Effect Values")]
    public float value1;
    public float value2;
    public float duration;

    [Header("Optional Visuals / Sounds")]
    public GameObject visualEffect;
    public AudioClip activationSound;
}
