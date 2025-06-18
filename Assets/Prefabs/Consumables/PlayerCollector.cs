using UnityEngine;

public class PlayerCollector : MonoBehaviour, ICollector
{
    public void Collect(CollectibleData data)
    {
        if (data != null)
        {
            GetComponent<PlayerXP>()?.GainXP(data.xpAmount);
            Debug.Log($"Collected: {data.itemName}, Gained {data.xpAmount} XP");
        }
    }
}
