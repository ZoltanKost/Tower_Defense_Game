using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip menuMusic;
    public AudioClip waveMusic;
    public AudioClip lostMusic;
    public float fadeDuration = 1f;
    [SerializeField] AudioSource source;
    public void PlayMenuMusic()
    {
        Tween tween = source.DOFade(0f,fadeDuration);
        tween.OnComplete(() => {
            source.Stop();
            source.clip = menuMusic;
            source.volume = 1f;
            source.Play();
        }); 
    }
    public void PlayLostMusic()
    {
        source.Stop();
        source.clip = lostMusic;
        source.Play();
    }
    public void PlayWaveMusic()
    {
        Tween tween = source.DOFade(0f, fadeDuration);
        tween.OnComplete(() => {
            source.Stop();
            source.clip = waveMusic;
            source.Play();
            source.volume = 1f;
        });
    }
}
