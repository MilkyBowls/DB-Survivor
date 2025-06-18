using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyWaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WeightedEnemy
    {
        public GameObject enemyPrefab;
        public float weight = 1f;
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount = 5;
        public float spawnInterval = 1f;
        public int enemiesPerInterval = 1;

        public List<WeightedEnemy> enemies;

        public bool useSpawnPattern = false;
        public SpawnPatternSO spawnPattern;
    }

    public List<Wave> waves;
    public Transform player;
    public float spawnRadius = 10f;
    public float safeZoneRadius = 3f;

    private int currentWave = 0;
    private bool spawning = false;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (currentWave < waves.Count)
        {
            Wave wave = waves[currentWave];
            spawning = true;

            int spawned = 0;
            while (spawned < wave.enemyCount)
            {
                for (int i = 0; i < wave.enemiesPerInterval && spawned < wave.enemyCount; i++)
                {
                    SpawnEnemy(wave, spawned);
                    spawned++;
                }

                yield return new WaitForSeconds(wave.spawnInterval);
            }

            spawning = false;
            currentWave++;

            // Optional: Wait for all enemies to die before next wave
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
        }
    }

    private void SpawnEnemy(Wave wave, int spawnIndex)
    {
        GameObject enemyPrefab = GetWeightedRandomEnemy(wave.enemies);
        if (enemyPrefab == null) return;

        Vector2 spawnPos;

        if (wave.useSpawnPattern && wave.spawnPattern != null && wave.spawnPattern.spawnOffsets.Count > 0)
        {
            int patternIndex = wave.spawnPattern.useSequential
                ? spawnIndex % wave.spawnPattern.spawnOffsets.Count
                : Random.Range(0, wave.spawnPattern.spawnOffsets.Count);

            Vector2 offset = wave.spawnPattern.spawnOffsets[patternIndex];
            spawnPos = (Vector2)player.position + offset;
        }
        else
        {
            spawnPos = GetValidSpawnPosition();
        }

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private GameObject GetWeightedRandomEnemy(List<WeightedEnemy> enemies)
    {
        float totalWeight = 0f;
        foreach (var enemy in enemies) totalWeight += enemy.weight;

        float randomWeight = Random.Range(0f, totalWeight);
        float current = 0f;

        foreach (var enemy in enemies)
        {
            current += enemy.weight;
            if (randomWeight <= current) return enemy.enemyPrefab;
        }

        return null;
    }

    private Vector2 GetValidSpawnPosition()
    {
        Vector2 spawnPos;
        int attempts = 0;

        NNConstraint constraint = NNConstraint.Default;
        constraint.constrainWalkability = true;
        constraint.walkable = true;
        constraint.constrainArea = true;
        constraint.constrainDistance = true;

        do
        {
            spawnPos = (Vector2)player.position + Random.insideUnitCircle.normalized * spawnRadius;

            var nearest = AstarPath.active.GetNearest(spawnPos, constraint);
            bool valid = nearest.node != null && nearest.node.Walkable && nearest.position == (Vector3)spawnPos;

            bool tooClose = Vector2.Distance(spawnPos, player.position) < safeZoneRadius;

            if (valid && !tooClose)
                return spawnPos;

            attempts++;
        }
        while (attempts < 50);

        return spawnPos;
    }


}
