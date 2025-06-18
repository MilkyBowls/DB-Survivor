using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemies/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public int maxHealth = 3;
    public int damage = 5;
    public float moveSpeed = 2f;
    public GameObject deathEffect;
}