using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    // Persistent Shake Variables (e.g. for charging)
    private float shakeDuration = 0f;
    [Header("Camera Shake Settings")]
    public float shakeMagnitude = 0.05f;
    public float shakeSpeed = 25f;

    // Impulse Shake Variables (e.g. for explosions)
    private float impulseShakeDuration = 0f;
    private float impulseShakeMagnitude = 0f;
    private float impulseShakeSpeed = 0f;

    // Zoom Variables
    public float defaultZoom = 5f;
    public float zoomInAmount = 4f;
    public float zoomSpeed = 5f;
    private Camera cam;
    private bool isCharging;
    public bool lockToPlayer = false;

    public static CameraFollow Instance { get; private set; }

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Smooth camera follow
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);

        // Combined shake offset from both persistent and impulse shakes
        Vector3 totalShakeOffset = Vector3.zero;

        // Persistent shake (charging)
        if (shakeDuration > 0)
        {
            float shakeX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f * shakeMagnitude;
            float shakeY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f * shakeMagnitude;
            totalShakeOffset += new Vector3(shakeX, shakeY, 0f);
        }

        // Impulse shake (e.g., explosion)
        if (impulseShakeDuration > 0)
        {
            float impulseX = (Mathf.PerlinNoise(Time.time * impulseShakeSpeed, 1f) - 0.5f) * 2f * impulseShakeMagnitude;
            float impulseY = (Mathf.PerlinNoise(1f, Time.time * impulseShakeSpeed) - 0.5f) * 2f * impulseShakeMagnitude;
            totalShakeOffset += new Vector3(impulseX, impulseY, 0f);

            impulseShakeDuration -= Time.deltaTime;
        }

        // Apply shake
        transform.position += totalShakeOffset;

        // Handle zoom effect
        float targetZoom = isCharging ? zoomInAmount + Mathf.Sin(Time.time * 10f) * 0.2f : defaultZoom;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

        if (lockToPlayer)
        {
            desiredPosition = target.position + offset;
        }
        else
        {
            desiredPosition = Vector3.Lerp(
                transform.position,
                target.position + offset,
                smoothSpeed * Time.unscaledDeltaTime
            );
        }

        transform.position = new Vector3(desiredPosition.x, desiredPosition.y, transform.position.z);
    }

    // Persistent effects
    public void StartChargingEffects()
    {
        isCharging = true;
        shakeDuration = Mathf.Infinity;
    }

    public void StopChargingEffects()
    {
        isCharging = false;
        shakeDuration = 0f;
    }

    // Impulse shake for quick bursts like explosions
    public void TriggerImpulseShake(float duration, float magnitude, float speed)
    {
        impulseShakeDuration = duration;
        impulseShakeMagnitude = magnitude;
        impulseShakeSpeed = speed;
    }

    public void SaibamanExplosion()
    {
        TriggerImpulseShake(0.2f, 0.1f, 10f); // adjust as needed
    }

    public void TemporaryZoom(float targetSize, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomCoroutine(targetSize, duration));
    }

    private IEnumerator ZoomCoroutine(float targetSize, float duration)
    {
        float startSize = cam.orthographicSize;
        float t = 0f;

        while (t < duration)
        {
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t / duration);
            t += Time.unscaledDeltaTime; // unaffected by time scale
            yield return null;
        }

        cam.orthographicSize = targetSize;

        // Optional: zoom back out after delay
        yield return new WaitForSecondsRealtime(0.4f); // delay after zoom-in

        t = 0f;
        while (t < duration)
        {
            cam.orthographicSize = Mathf.Lerp(targetSize, defaultZoom, t / duration);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        cam.orthographicSize = defaultZoom;
    }

}
