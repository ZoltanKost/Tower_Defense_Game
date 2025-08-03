using System;
using UnityEngine;

public class AudioPooling : MonoBehaviour
{
    [SerializeField] private AudioSource prefab;
    AudioSource[] audioSources;
    int count;
    public void Awake()
    {
        audioSources = new AudioSource[8];
        for (int i = 0; i < 8; i++)
        {
            audioSources[i] = Instantiate(prefab,transform);
        }
    }

    public void Update()
    {
        for(int i = 0; i < count; i++)
        {
            if (!audioSources[i].isPlaying) {
                Remove(i);
                i--;
            };
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if(count >= audioSources.Length)
        {
            Resize();
        }
        audioSources[count].clip = clip;
        if (audioSources[count].isPlaying) Debug.LogError($"Source {count} is still playing");
        audioSources[count].pitch = UnityEngine.Random.Range(0.3f, 0.6f);
        audioSources[count].Play();
        count++;
    }
    public void PlaySoundNormalPitch(AudioClip clip)
    {
        if (count >= audioSources.Length)
        {
            Resize();
        }
        audioSources[count].clip = clip;
        if (audioSources[count].isPlaying) Debug.LogError($"Source {count} is still playing");
        audioSources[count].pitch = 1f;
        audioSources[count].Play();
        count++;
    }

    public void Remove(int ID)
    {
        AudioSource temp = audioSources[ID];
        audioSources[ID] = audioSources[--count];
        audioSources[count] = temp;
        audioSources[count].Stop();
    }

    void Resize()
    {
        Array.Resize(ref audioSources, count * 2);
        for (int i = count; i < audioSources.Length; i++)
        {
            audioSources[i] = Instantiate(prefab);
        }
    }
}
