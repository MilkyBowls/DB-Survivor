using UnityEngine;
using UnityEngine.InputSystem;

public class FirePointController : MonoBehaviour
{
    public Transform player;
    public float maxRadius = 1.0f;
    public Vector3 offsetFromPlayerCenter = new Vector3(0, 0.5f, 0); // tweak this based on your sprite offset

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

        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        // Calculate direction from player to mouse
        Vector3 playerCenter = player.position + offsetFromPlayerCenter;
        Vector3 direction = (mousePos - playerCenter).normalized;

        // Clamp distance to max radius
        Vector3 offset = direction * maxRadius;

        // Set fire point position relative to adjusted center
        transform.position = playerCenter + offset;

        // Face the mouse direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
