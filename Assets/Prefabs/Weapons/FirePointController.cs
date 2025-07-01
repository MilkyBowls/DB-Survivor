using UnityEngine;
using UnityEngine.InputSystem;

public class FirePointController : MonoBehaviour
{
    public Transform player;
    public float maxRadius = 1.0f;
    public float minInputRadius = 0.25f;
    public Vector3 offsetFromPlayerCenter = new Vector3(0, 0.5f, 0);

    private Vector3 lastValidDirection = Vector3.right;

    // ✅ Add this for external weapon scripts to use
    public bool IsAimingTooClose { get; private set; } = false;

    public Vector3 LastValidDirection => lastValidDirection;
    public Vector3 PlayerCenter => player.position + offsetFromPlayerCenter;

    void Awake()
    {
        if (player == null)
        {
            PlayerController found = FindFirstObjectByType<PlayerController>();
            if (found != null) player = found.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        Vector3 playerCenter = player.position + offsetFromPlayerCenter;
        Vector3 toMouse = mousePos - playerCenter;
        float distance = toMouse.magnitude;

        Vector3 direction;

        // ✅ Set aiming state for use by weapons
        IsAimingTooClose = distance <= minInputRadius;

        if (!IsAimingTooClose)
        {
            direction = toMouse.normalized;
            lastValidDirection = direction;
        }
        else
        {
            direction = lastValidDirection;
        }

        transform.position = playerCenter + direction * maxRadius;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        Vector3 center = player.position + offsetFromPlayerCenter;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, maxRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, minInputRadius);
    }
}
