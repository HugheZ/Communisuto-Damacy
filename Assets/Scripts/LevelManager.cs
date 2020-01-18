using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //Instance
    private static LevelManager _instance;

    //Singleton awake
    private void Awake()
    {
        if (_instance != null && this != _instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    //singleton instance getter
    public static LevelManager Instance { get { return _instance; } }


    //GAMEPLAY VALUES
    public int maxTime; //maximum time in seconds
    public float slowTimeScale; //timescale to slow the game by on grow
    public float startSize; //size of the starting ball
    public GameObject player; //player reference

    //UI values
    public Canvas UI; //ui reference, used for voiceover
    public Image growthSymbol; //symbol used for growth
    public Text size; //size of the player
    public Text clock; //the game timer
    public Image clockBackground; //background reference
    public Color minuteLeftColor; //color to change teh clock to on minute left
    bool hitMinuteFlag; //flag for if it is the first time we hit a minute left
    public VoiceOver voice; //voiceover reference
    public GameObject dashLines; //dash lines holder
    public GameObject winAnimator; //ending scene animator
    bool paused; //is the game paused
    bool gameOver; //is the game over
    public GameObject pausePanel; //panel to show when pausing

    //SOUND VALUES
    public List<AudioClip> voices; //sounds for voice over
    public AudioSource voiceOver; //voiceover sound
    public AudioSource sound; //sound source

    //WIN VALUES
    float length; //current length of the ball
    public float minimumLengthToWin; //how big needed to be to win
    public float winLengthScore; //how many points you get upon winning
    public float winLengthOverScore; //how many points you get for each length unit above win threshold


    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        gameOver = false;
        hitMinuteFlag = false;
        BeginGame();
        length = startSize;
    }

    // Update is called once per frame
    void Update()
    {
        //if hit escape, pause, else unpause
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            if (!paused) Pause();
            else UnPause();
        }
    }

    //Pauses the game
    void Pause()
    {
        Time.timeScale = 0;
        sound.Pause();
        pausePanel.SetActive(true);
        paused = true;
    }

    //Unpauses the game
    void UnPause()
    {
        Time.timeScale = 1;
        sound.UnPause();
        pausePanel.SetActive(false);
        paused = false;
    }

    //Begins play logic
    void BeginGame()
    {
        //set clock and begin countdown
        SetClockTime(maxTime);
        StartCoroutine(CountDown());
    }

    //Sets the clock to the max time
    void SetClockTime(int time)
    {
        //if more than a minute left, display minutes left
        if (time > 60)
        {
            clock.text = string.Format("{0}", time / 60);
        }
        else
        {
            //if first time, set clock color
            if (!hitMinuteFlag)
            {
                clockBackground.color = minuteLeftColor;
                hitMinuteFlag = true;
            }
            //always show seconds left
            clock.text = string.Format("{0}", time);
        }
    }

    //counts down the game timer
    IEnumerator CountDown()
    {
        //get local time counter
        int currTime = maxTime;

        //while time left
        while(currTime > 0)
        {
            //wait a second
            yield return new WaitForSeconds(1);
            
            //decrement a second
            currTime--;

            //set clock time
            SetClockTime(currTime);
        }

        //timeout, end game
        EndGame();
    }

    //updates the score by size of player by minimum size
    public void UpdateSize(float radius)
    {
        this.length = 2 * (radius * startSize);
        int length = Mathf.FloorToInt(2 * (radius * startSize));
        size.text = string.Format("{0:0.##} cm, {1:0.##} mm", length % 100, Mathf.FloorToInt((this.length - length) * 100));
        growthSymbol.gameObject.transform.localScale = new Vector3(1, 1, 1) * this.length / minimumLengthToWin;
    }

    //plays animation upon reaching new threshold
    public void PassedSizeThreshold()
    {
        try
        {
            Messenger.Broadcast(GameEvents.INCREASED_SIZE);
        }
        catch
        {
            Debug.Log("No Mouse");
        }

        //play voiceover
        voice.StartVoiceover();

        //play sound
        voiceOver.PlayOneShot(voices[Random.Range(0, voices.Count - 1)]);
    }

    //plays animation for dashing
    public void StartDashAnim()
    {
        dashLines.SetActive(true);
    }

    //stops animation for dashing
    public void StopDashAnim()
    {
        dashLines.SetActive(false);
    }

    //ends the game
    void EndGame()
    {
        //set game over
        gameOver = true;

        //stop music
        sound.Stop();

        //save data
        PlayerPrefs.SetFloat("FINAL_SIZE", length);

        //disable player movemnt
        player.GetComponent<BallMover>().enabled = false;
        player.GetComponent<ExtendedMovement>().enabled = false;
        player.GetComponent<Pickup>().enabled = false;
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Camera.main.GetComponentInParent<CameraMover>().enabled = false;

        //set ending animation 
        winAnimator.SetActive(true);
    }
}
