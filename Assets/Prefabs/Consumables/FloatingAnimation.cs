using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 1f;

    [Header("Rotation (optional)")]
    public bool rotate = false;
    public float rotationSpeed = 50f;

    private Vector3 startPos;

    public bool disableWhileMagnetized = true;
    private bool isMagnetized = false;

    public void SetMagnetized(bool value)
    {
        isMagnetized = value;
    }

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        if (disableWhileMagnetized && isMagnetized) return;

        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = startPos + new Vector3(0f, yOffset, 0f);

        if (rotate)
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
