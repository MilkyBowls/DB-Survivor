using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnPattern", menuName = "Wave Spawning/Spawn Pattern")]
public class SpawnPatternSO : ScriptableObject
{
    [Tooltip("Offsets from the player where enemies will spawn.")]
    public List<Vector2> spawnOffsets;

    [Tooltip("Should the pattern spawn in sequence or random from list?")]
    public bool useSequential = false;
}
