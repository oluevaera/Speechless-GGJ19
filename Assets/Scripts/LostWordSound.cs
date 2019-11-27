using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostWordSound : MonoBehaviour
{

    AudioSource lostWordS;//ADDED NOW
    public AudioClip lostWord;//ADDED NOW
    // Start is called before the first frame update
    void Start()
    {
        lostWordS = GetComponent<AudioSource>();
    }

    public void playClip()
    {
        lostWordS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
