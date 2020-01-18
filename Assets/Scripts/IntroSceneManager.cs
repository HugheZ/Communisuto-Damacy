using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This intro scene manager handles the first two scenes, an opening scene
/// and an info scene, due to their similarities. It allows the player to
/// input any button to skip segments and to start the game proper.
/// </summary>
/// NOTE: reused from Games 1 Project 1, 1T Lions vs the Sun
public class IntroSceneManager : MonoBehaviour {


    public Image panel;         //background for second press start prompt
    public Text info;           //info text
    public float time;          //time to wait before blinking
    private float zeroTime;     //time value for counting
    private bool isRunning;     //is the info text running
    private bool isOpenForInput;//if the text is finished and ready for next scene input
    private string infoTxt;     //info text actual text
    public float typeDelay;     //delay between typing characters
    private AudioSource talkSound;//the sound of talking used on the info screen
    private bool soundFlag;     //flag to play transmission sound
    public Animator anim; //animator to play when opening box

    // Use this for initialization
    void Start () {
        talkSound = GetComponent<AudioSource>();
        isRunning = false;
        isOpenForInput = false;
        soundFlag = true;
        zeroTime = Time.time;

	}
	
	// Update is called once per frame
	void Update () {
            if (!isRunning)
            {
                infoTxt = info.text;
                info.text = "";
                StartCoroutine("StartText");
            }
            //if there's input and the text is done, start animation and destroy
            //this dummy manager
            if(Input.anyKeyDown && isOpenForInput)
            {
                anim.SetTrigger("OPEN");
            }
            //if there's input and the text is still going, finish it
            else if (Input.anyKeyDown && !isOpenForInput)
            {
                info.text = infoTxt;
                isOpenForInput = true;
                StopCoroutine("StartText");
            }
            //flicker panel if possible
            if (isOpenForInput && Time.time - zeroTime > time)
            {
                zeroTime = Time.time;
            panel.gameObject.SetActive(!panel.gameObject.activeInHierarchy);
            }
    }

    //Prints text one letter at a time for the info screen and plays a
    //talk sound at a random pitch
    private IEnumerator StartText()
    {
        //read out text and wait
        int i;
        isRunning = true;
        for (i = 0; i < infoTxt.Length+1; i++)
        {
            //play random sound if we haven't hit the '=' yet
            if (i < infoTxt.Length && infoTxt[i] == '=') soundFlag = false;
            if (soundFlag && i % 3 == 0)
            {
                talkSound.pitch = Random.Range(1f, 2f);
                talkSound.Play();
            }
            info.text = infoTxt.Substring(0, i);
            yield return new WaitForSeconds(typeDelay);
        }
        //enable start input
        isOpenForInput = true;
    }

}
