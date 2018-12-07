using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {

	AudioSource myAudio;
	// Use this for initialization
	void Start () {
		myAudio= GetComponent<AudioSource>();
		Invoke("playAudio", 6.0f);
	}
	void playAudio()
	{
		myAudio.Play();
	}
	// Update is called once per frame
	void Update () {
		
	}
}
