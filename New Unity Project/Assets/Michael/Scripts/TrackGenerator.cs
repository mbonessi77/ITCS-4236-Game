using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] tracks;
    [SerializeField] private GameObject currentTrack;

	// Use this for initialization
	void Start ()
    {
        //Loop for testing
        for(int i = 0; i < 10; i++)
        {
            SpawnTrack();
        }
	}
    
    //Picks a random piece of track to spawn at the end of the current track
    void SpawnTrack()
    {
        int rand = Random.Range(0, tracks.Length);

        currentTrack = Instantiate(tracks[rand], GetGrandchildPosition(currentTrack), Quaternion.identity);
    }

    //Gets the position of a child's child to clean the code up a little
    Vector3 GetGrandchildPosition(GameObject obj)
    {
        return obj.transform.GetChild(0).transform.GetChild(0).position;
    }
}
