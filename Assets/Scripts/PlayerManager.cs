using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{   
	Animator animator;

	public Rigidbody2D rb2d;
	public GameObject weapon;
   
	bool canFire = true;
	bool canJump = false;

	double zeroEpsilon = 0.0;
	Vector2 move;

	// Use this for initialization
	void Start()
	{
		animator = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		move = Vector2.zero;
	}

	// Update is called once per frame
	void Update()
	{
		move = rb2d.velocity;
		move.x = Input.GetAxis("Horizontal") * 10f;

		if (move.x < 0f) {
			transform.localScale = new Vector3(-1, 1, 1);
		} 
		if (move.x > 0f) {
			transform.localScale = new Vector3(1, 1, 1);
		}

		if (Input.GetKeyDown(KeyCode.UpArrow) && canJump)
        {
			canJump = false;
			move.y = 20f;
        }
        

		if (Mathf.Abs(rb2d.velocity.x) < 1f) {
			animator.SetInteger("State", 0);
		} else {
			animator.SetInteger("State", 1);
		}

		rb2d.velocity = move;

		if (Input.GetKeyDown(KeyCode.Space) && canFire) {
            weapon.GetComponent<Animator>().SetBool("isFiring", true);
			canFire = false;
        }

        // Trigger must be released to fire again
		canFire |= Input.GetKeyUp(KeyCode.Space);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		canJump = true;	
	}
}
