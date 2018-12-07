using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonScript : MonoBehaviour
{

    public static SingletonScript self;

    // Use this for initialization
    void Start()
    {
        if (self == null)
        {
            self = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
