using UnityEngine;
using System.Collections;
public class Hitsound : MonoBehaviour {
    void OnCollisionEnter2D(Collision2D hit)
    {
        if(hit.gameObject.tag == “House“ )
        GetComponent<AudioSource>().Play();
        
    
         
    }
}
