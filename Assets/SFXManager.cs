using UnityEngine;
using UnityEngine.Rendering;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("SFX Clips")]
    public AudioClip kiBlast;
    public AudioClip kiBlastImpact;
    public AudioClip kiChargeLoop;
    public AudioClip SaibamanExplosion;
    public AudioClip SSJ1Transform;
    public AudioClip PowerDown;

    private AudioSource audioSource;
    private AudioSource chargeLoopSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        // Setup loop audio source
        chargeLoopSource = gameObject.AddComponent<AudioSource>();
        chargeLoopSource.playOnAwake = false;
        chargeLoopSource.volume = 0.3f;
    }

    public void Play(AudioClip clip)
    {
        if (clip != null)
        {
            float volume = audioSource.volume;
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void StartKiChargeLoop()
    {
        if (kiChargeLoop != null && !chargeLoopSource.isPlaying)
        {
            chargeLoopSource.clip = kiChargeLoop;
            chargeLoopSource.loop = true;
            chargeLoopSource.Play();
        }
    }

    public void StopKiChargeLoop()
    {
        if (chargeLoopSource != null && chargeLoopSource.isPlaying)
        {
            chargeLoopSource.Stop();
        }
    }
}
