using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour {

    public AudioClip hitTheFloor;

    private AudioSource source;

    public void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        source.PlayOneShot(hitTheFloor, 1f);   
    }
}
