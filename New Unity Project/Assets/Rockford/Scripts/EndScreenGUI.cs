using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreenGUI : MonoBehaviour {

    [SerializeField]
    private Text timerText;
	
	// Update is called once per frame
	void Update ()
    {
        //display the final time
        timerText.text = "Final Time: " + GameManager.timeMin + ":" + GameManager.timeSec.ToString("00");
    }
}
