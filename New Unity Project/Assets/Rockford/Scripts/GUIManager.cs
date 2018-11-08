using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    private Text timerText, speedText;
    private int seconds, minutes;
    [SerializeField]
    private RollMovement playerScript;

    // Use this for initialization
    void Start()
    {
        //initialize seconds and minutes to zero on start
        seconds = 0;
        minutes = 0;

        //start timer
        StartCoroutine(TimeTracker());
    }

    //time tracker coroutine
    IEnumerator TimeTracker()
    {
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
        if(seconds == 60)
        {
            //add a minute
            minutes += 1;
            //reset seconds
            seconds = 0;
        }

        //display the timer
        timerText.text = "Time: " + minutes + ":" + seconds.ToString("00"); ;

        //display current speed
        speedText.text = "" + Mathf.Floor(playerScript.GetCurrentSpeed() * 1.5f) + "mph";
    }
}
