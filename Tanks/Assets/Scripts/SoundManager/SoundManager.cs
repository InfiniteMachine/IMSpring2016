using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
    public static SoundManager instance;

    public GameObject oneShotAudio;
    public GameObject backgroundAudio;

    public AudioClip[] backgroundClips;
    private int[] maxPlaying;
    private AudioSource[] background;


    public AudioClip[] soundEffects;
    [Range(0f, 1f)]
    public float globalVolume = 1;
    
	void Start () {
        if (instance == null)
            instance = this;
        background = new AudioSource[backgroundClips.Length];
        maxPlaying = new int[backgroundClips.Length];
        for(int i = 0; i < backgroundClips.Length; i++)
        {
            GameObject go = Instantiate(backgroundAudio);
            go.transform.SetParent(transform);
            go.transform.position = Vector3.zero;
            AudioSource aSource = go.GetComponent<AudioSource>();
            aSource.clip = backgroundClips[i];
            aSource.volume = globalVolume;
            background[i] = aSource;
            maxPlaying[i] = 0;
        }
	}

    public void SetGlobalVolume(float volume)
    {
        for (int i = 0; i < background.Length; i++)
        {
            float baseVolume = background[i].volume / globalVolume;
            background[i].volume = baseVolume * volume;
        }
        globalVolume = volume;
    }

    public void SetBackgroundVolume(string name, float volume)
    {
        for(int i = 0; i < background.Length; i++)
        {
            if (background[i].clip.name == name)
            {
                background[i].volume = volume * globalVolume;
                return;
            }
        }
        Debug.Log("SoundManager: Background '" + name + "' does not exits.");
    }

    public void PlayOneShot(string name)
    {
        PlayOneShot(name, globalVolume);
    }

    public void PlayOneShot(string name, float volume)
    {
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundEffects[i].name == name)
            {
                GameObject oneShot = Instantiate(oneShotAudio);
                oneShot.transform.SetParent(transform);
                oneShot.transform.position = Vector3.zero;
                OneShotAudio audio = oneShot.GetComponent<OneShotAudio>();
                audio.PlayOneShot(soundEffects[i], volume * globalVolume);
                return;
            }
        }
        Debug.Log("SoundManager: OneShot '" + name + "' does not exits.");
    }

    public void StopAll()
    {
        foreach(Transform t in transform)
        {
            if (t.name.Contains("OneShot"))
                Destroy(t.gameObject);
        }
        foreach (AudioSource aSource in background)
            aSource.Stop();
    }

    public void StopBackground(string name)
    {
        for(int i = 0; i < background.Length; i++)
        {
            if (background[i].clip.name == name)
            {
                if (maxPlaying[i] > 0)
                {
                    if (maxPlaying[i] == 1)
                        background[i].Stop();
                    maxPlaying[i]--;
                }
                return;
            }
        }
        Debug.Log("SoundManager: Background '" + name + "' does not exits.");
    }

    public void PlayBackground(string name)
    {
        for (int i = 0; i < background.Length; i++)
        {
            if (background[i].clip.name == name)
            {
                if(maxPlaying[i] == 0)
                    background[i].Play();
                maxPlaying[i]++;
                return;
            }
        }
        Debug.Log("SoundManager: Background '" + name + "' does not exits.");
    }
}