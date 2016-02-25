using UnityEngine;
using System.Collections;

public class OneShotAudio : MonoBehaviour {
    public void PlayOneShot(AudioClip aClip, float volume)
    {
        AudioSource aSource = GetComponent<AudioSource>();
        aSource.clip = aClip;
        aSource.Play();
        aSource.volume = volume;
        Destroy(gameObject, aClip.length);
    }
}
