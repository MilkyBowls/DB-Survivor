using UnityEngine;
using System.Collections.Generic;

public class CollectibleObject : MonoBehaviour
{
    public CollectibleData data;
    public float magnetSpeed = 6f;

    private Transform magnetTarget = null;
    private bool isMagnetized = false;

    public static List<CollectibleObject> ActiveCollectibles = new List<CollectibleObject>();

    void OnEnable() => ActiveCollectibles.Add(this);
    void OnDisable() => ActiveCollectibles.Remove(this);

    public void StartMagnetize(Transform target)
    {
        magnetTarget = target;
        isMagnetized = true;
        GetComponent<FloatingAnimation>()?.SetMagnetized(true);
    }

    public void StopMagnetize()
    {
        magnetTarget = null;
        isMagnetized = false;
        GetComponent<FloatingAnimation>()?.SetMagnetized(false);
    }

    void Update()
    {
        if (isMagnetized && magnetTarget != null)
        {
            Vector3 dir = (magnetTarget.position - transform.position).normalized;
            transform.position += dir * magnetSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var collector = other.GetComponent<ICollector>();
        if (collector != null)
        {
            collector.Collect(data);
            Destroy(gameObject);
        }
    }
}
