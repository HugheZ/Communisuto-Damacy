using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour {

    public bool isIntroScene;
    public AudioSource source;
    public AudioClip endSalute; //ending salute
    public AudioClip woosh; //woosh soud
    AsyncOperation ao; //async load operation

    //On start, if not open scene, begin loading async the level
    private void Start()
    {
        if (isIntroScene)
        {
            BeginLoad();
        }
    }

    //plays sound associated if possible
    public void PlayEndSalute()
    {
        try
        {
            source.PlayOneShot(endSalute);
        }
        catch
        {
            Debug.Log("no sound or source");
        }
    }

    //plays sound associated if possible
    public void PlayWoosh()
    {
        try
        {
            source.PlayOneShot(woosh);
        }
        catch
        {
            Debug.Log("no sound or source");
        }
    }

    //removes player
    public void RemovePlayer()
    {
        LevelManager.Instance.player.SetActive(false);
    }

    //ends game, goes to end scene
    public void EndGame()
    {
        SceneManager.LoadScene("EndScene");
    }

    //Loads intro scene to move to game scene
    public void StartGame()
    {
        SceneManager.LoadScene("InfoScene");
    }

    //starts loading of main scene
    public void BeginLoad()
    {
        ao = SceneManager.LoadSceneAsync("MainScene");
        ao.allowSceneActivation = false;
    }

    //Loads the actual game scene
    public void BeginPlay()
    {
        ao.allowSceneActivation = true;
    }

    //Returns to the main menu and resets timescale to reset for new playthrough
    public void MainMenu()
    {
        SceneManager.LoadScene("OpenScene");
        Time.timeScale = 1;
    }
	
	//Quits the game
    public void QuitGame()
    {
        print("App Quit");
        Application.Quit();
    }
}
