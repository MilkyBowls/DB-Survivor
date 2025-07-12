using Pathfinding;
using UnityEngine;

public class TargetSetter : MonoBehaviour
{
    void Start()
    {
        var setter = GetComponent<AIDestinationSetter>();
        var player = GameObject.FindGameObjectWithTag("Player");
        if (setter != null && player != null)
        {
            setter.target = player.transform;
        }

        var ai = GetComponent<AIPath>();
        if (ai != null)
        {
            // Offset each enemy's repath slightly
            ai.repathRate = Random.Range(0.1f, 0.3f);
        }
    }
}