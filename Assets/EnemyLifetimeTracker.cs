using UnityEngine;

public class EnemyLifetimeTracker : MonoBehaviour
{
    private static int activeEnemies = 0;
    public static int ActiveEnemies => activeEnemies;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCounter()
    {
        activeEnemies = 0;
    }

    public static void EnsureTracking(GameObject enemy)
    {
        if (enemy == null)
        {
            return;
        }

        if (!enemy.TryGetComponent<EnemyLifetimeTracker>(out _))
        {
            enemy.AddComponent<EnemyLifetimeTracker>();
        }
    }

    private void OnEnable()
    {
        activeEnemies++;
    }

    private void OnDisable()
    {
        if (activeEnemies > 0)
        {
            activeEnemies--;
        }
    }
}
