public interface IDamageable
{
    void TakeDamage(int amount, float delayBeforeDeath);
    void TakeDamage(int amount);

    // ✅ Add these for the KiBlast upgrade system
    int CurrentHealth { get; }
    int MaxHealth { get; }

    void ApplyStatusEffect(StatusEffect effectType, float duration);
}

public enum StatusEffect
{
    Burn,
    Stun
}