using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    private Text timerText, speedText, countdownText;
    [SerializeField]
    private AudioSource gunShotAudioSource;
    private static int seconds, minutes, countdown;
    [SerializeField]
    private RollMovement playerScript;

    // Use this for initialization
    void Start()
    {
        seconds = 0;
        minutes = 0;
        countdown = 6;
        //start timer
        //StartCoroutine(TimeTracker());
        StartCoroutine(RaceStartCountdown());
    }

    //countdown to start of race
    IEnumerator RaceStartCountdown()
    {
        bool countingDown = true;
        while (countingDown)
        {
            yield return new WaitForSeconds(1);
            countdown--;
            if (countdown == 0)
            {
                countingDown = false;
            }
        }

        GameManager.raceIsStarting = false;
        StartCoroutine(TimeTracker());
    }

    //time tracker coroutine
    IEnumerator TimeTracker()
    {
        //play the gun shot sound to start race
        gunShotAudioSource.Play();

        //continue to add a second every second
        while (true)
        {
            yield return new WaitForSeconds(1);
            seconds++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if reached 60 seconds
        if (seconds == 60)
        {
            //add a minute
            minutes += 1;
            //reset seconds
            seconds = 0;
        }

        //display the timer
        timerText.text = "Time: " + minutes + ":" + seconds.ToString("00");

        //display current speed
        speedText.text = "" + Mathf.Floor(playerScript.GetCurrentSpeed() * 1.5f) + "mph";

        //display countdown text
        if (countdown == 3)
        {
            countdownText.color = Color.red;
            countdownText.text = "Ready!";
        }
        else if (countdown == 2)
        {
            countdownText.color = Color.yellow;
            countdownText.text = "Set!";
        }
        else if (countdown == 1)
        {
            countdownText.color = Color.green;
            countdownText.text = "Go!";
        }
        else
        {
            countdownText.text = "";
        }

        //when player hits the finish line
        if(GameManager.raceFinished)
        {
            //set the global time variables to the correct time and then load the end screen
            GameManager.timeMin = minutes;
            GameManager.timeSec = seconds;
            SceneManager.LoadScene("EndScreen");
        }
    }
}
