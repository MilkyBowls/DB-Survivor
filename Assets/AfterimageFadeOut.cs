using UnityEngine;

public class AfterimageFadeOut : MonoBehaviour
{
    public float lifetime = 0.5f;
    public float fadeSpeed = 2f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        Color c = sr.color;
        c.a -= fadeSpeed * Time.deltaTime;
        sr.color = c;
    }
}
