using UnityEngine;
using Pathfinding;

public class SwarmChase : MonoBehaviour
{
    [Header("Swarm Behavior")]
    public float followOffsetRadius = 0.5f;
    public float repathIntervalMin = 0.5f;
    public float repathIntervalMax = 1.5f;

    [Header("AIPath Settings")]
    public float minRepathRate = 0.1f;
    public float maxRepathRate = 0.3f;

    private Transform player;
    private Transform dynamicTarget;

    void Start()
    {
        // Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        // Set up AIDestinationSetter target
        var setter = GetComponent<AIDestinationSetter>();
        if (setter != null)
        {
            GameObject tempTarget = new GameObject("SwarmTarget");
            dynamicTarget = tempTarget.transform;
            dynamicTarget.parent = player;
            setter.target = dynamicTarget;

            RepositionSwarmTarget(); // initial offset
            StartCoroutine(RepathLoop());
        }

        // Randomize how often this agent recalculates its path
        var ai = GetComponent<AIPath>();
        if (ai != null)
        {
            ai.repathRate = Random.Range(minRepathRate, maxRepathRate);
        }
    }

    void RepositionSwarmTarget()
    {
        if (dynamicTarget != null && player != null)
        {
            Vector2 offset = Random.insideUnitCircle * followOffsetRadius;
            dynamicTarget.localPosition = offset;
        }
    }

    System.Collections.IEnumerator RepathLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(repathIntervalMin, repathIntervalMax);
            yield return new WaitForSeconds(waitTime);
            RepositionSwarmTarget();
        }
    }
}
