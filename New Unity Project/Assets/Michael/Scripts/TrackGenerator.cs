using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] tracks;
    [SerializeField] private GameObject currentTrack; //Track that has to start in the world to generate the rest of the track
    [SerializeField] private GameObject finishLine;
    [SerializeField] private GameObject wayPoints;

    private GameObject currentWaypoint;
    private int waypoint_int = 0;

	// Use this for initialization
	void Start ()
    {
        //clear Cache at start of level
        Waypoint_Cache.waypoints.Clear();
        GameManager.raceIsStarting = true;

        //Loop for testing
        for(int i = 0; i < 10; i++)
        {
            SpawnTrack(i);
        }
        currentTrack = Instantiate(finishLine, GetGrandchildPosition(currentTrack), Quaternion.identity); //Attach the finish line piece to the end
    }
    
    //Picks a random piece of track to spawn at the end of the current track
    void SpawnTrack(int trackNum)
    {
        int rand = Random.Range(0, tracks.Length);

        currentTrack = Instantiate(tracks[rand], GetGrandchildPosition(currentTrack), Quaternion.identity);
        currentWaypoint = Instantiate(wayPoints, GetGrandchildPosition(currentTrack), Quaternion.identity);

        if (trackNum == 4 || trackNum == 9)
        {
            Waypoint_Cache.waypoints.Add(waypoint_int, currentWaypoint);
            waypoint_int++;
            //print(Waypoint_Cache.waypoints.Count);
        }
    }

    //Gets the position of a child's child to clean the code up a little
    Vector3 GetGrandchildPosition(GameObject obj)
    {
        return obj.transform.GetChild(0).transform.GetChild(0).position;
    }
}
