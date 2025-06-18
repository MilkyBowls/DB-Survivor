using UnityEngine;

public class AudioAnimationEvents : MonoBehaviour
{
    public void PlaySSJ1TransformationSound()
    {
        SFXManager.Instance.Play(SFXManager.Instance.SSJ1Transform);
    }
}
