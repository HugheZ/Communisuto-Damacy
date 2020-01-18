using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOver : MonoBehaviour
{
    public List<AudioClip> possibleSounds; //possible sounds for the voiceover to make
    public AudioSource sound; //source of sound

    //Plays animation
    public void StartVoiceover()
    {
        gameObject.SetActive(true);
        if (possibleSounds.Count > 0)
        {
            sound.PlayOneShot(possibleSounds[Random.Range(0, possibleSounds.Count - 1)]);
        }
    }

    //disables object
    public void SetInactive()
    {
        gameObject.SetActive(false);
    }
}
