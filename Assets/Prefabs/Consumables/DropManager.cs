using UnityEngine;

public class DropManager : MonoBehaviour
{
    public DropTable dropTable;

    public void SpawnDrop()
    {
        if (dropTable == null || dropTable.drops.Count == 0) return;

        int totalWeight = 0;
        foreach (var entry in dropTable.drops)
            totalWeight += entry.weight;

        int roll = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var entry in dropTable.drops)
        {
            current += entry.weight;
            if (roll < current)
            {
                Instantiate(entry.item.prefab, transform.position, Quaternion.identity);
                break;
            }
        }
    }
}
