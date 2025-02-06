using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    public void Play()
    {
        source.pitch = Random.Range(0.8f, 1.2f);
        source.Play();
    }
}
