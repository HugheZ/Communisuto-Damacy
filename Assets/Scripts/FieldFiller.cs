using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldFiller : MonoBehaviour
{
    AudioSource endMusic; //Music to play
    public AudioClip russianAnthem; //Russian Anthem
    public AudioClip americanAnthem; //Star-Spangled Banner

    public GameObject goodEnd; //good end background
    public GameObject badEnd; //bad end background
    public Text winOrLoseField; //salute for winning or losing
    public Text scoreText; //text field to list score in
    float score; //how big the player got


    // Start is called before the first frame update
    void Start()
    {
        endMusic = GetComponent<AudioSource>();
        score = PlayerPrefs.GetFloat("FINAL_SIZE", 0f);

        //if lose
        if(score < GameValues.MIN_REQUIRED_LENGTH)
        {
            winOrLoseField.text = "Comrade! You have failed the motherland! The capitalists have launched their assault! All is lost!";
            badEnd.SetActive(true);
            endMusic.PlayOneShot(americanAnthem);
        }
        else //else win
        {
            winOrLoseField.text = "Comrade! You have proved our weapon concept viable. Prepare for your next mission, where you will take on our greatest threat...";
            goodEnd.SetActive(true);
            endMusic.PlayOneShot(russianAnthem);
        }

        //set score text
        int length = Mathf.FloorToInt(score);
        scoreText.text = string.Format("{0:0.##} cm, {1:0.##} mm", length % 100, Mathf.FloorToInt((score - length) * 100));
    }
}
