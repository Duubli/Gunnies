using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   
    public GameObject bunny;
    Bunny bunnyScript;
    GameObject playerBunny;

    Vector3 mousePosition;

    void Start()
    {
        playerBunny = Instantiate(bunny, transform.position, Quaternion.identity);
        gameObject.transform.parent = playerBunny.transform;
        bunnyScript = playerBunny.GetComponent<Bunny>();
        mousePosition = Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMousePosition();
        Controls();
    }

    private void Controls()
    {
        Vector2 velocity = bunnyScript.rb2D.velocity;
        velocity.x = Input.GetAxis("Horizontal") * 20f;

        if (Input.GetButton("Vertical") && bunnyScript.isJumping == false)
        {
            bunnyScript.isJumping = true;
            velocity.y = 20f;
        }

        if (Input.GetButtonDown("Fire"))
        {
            bunnyScript.GetComponentInChildren<Weapon>().Shoot();
        }

        if (Input.GetButton("Reload"))
        {
            bunnyScript.GetComponentInChildren<Weapon>().Reload();
        }

        bunnyScript.SetVelocity(velocity);
        bunnyScript.GetComponentInChildren<Weapon>().RotateWeapon(mousePosition);

        // Trigger must be released to fire again
        bunnyScript.GetComponentInChildren<Weapon>().canFire |= Input.GetButtonUp("Fire");
    }

    private void UpdateMousePosition()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
