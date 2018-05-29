using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public AudioClip hurtSound;

    public float health = 3;
    public GameObject bit;

    Animator animator;

    private AudioSource source;

    public void Start()
    {
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void FixedUpdate()
    {
        if (health < 3f){
            health += .01f;
        }
    }

    public void Hurt (float angle, float damage) {
        animator.SetTrigger("Hurt");
        source.PlayOneShot(hurtSound);
        health -= damage;
        if (health <= 0f) {
            animator.SetInteger("State", -1);
            gameObject.layer = 9;
            return;
        }
        for (int i = 0; i <= 50; i++) {
            GameObject bitClone = Instantiate(bit, transform.position, Quaternion.identity);
            Vector3 v = Quaternion.AngleAxis(angle + Random.Range(-45f, 45f), Vector3.forward) * Vector3.up;
            bitClone.GetComponent<Rigidbody2D>().velocity = v * Random.Range(-10f, 10f);
            Destroy(bitClone, 1);
        }
    }
}
