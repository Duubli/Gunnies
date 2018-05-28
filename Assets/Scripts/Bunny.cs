using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{   
	Animator animator;

    public Rigidbody2D rb2D;
	public SpriteRenderer sprite;
    public GameObject weapon;
    public GameObject casing;
    public GameObject enemy;
    public GameObject firingPoint;
   
	bool canFire = true;
    bool isJumping = true;

	// Use this for initialization
	void Start()
	{
		animator = GetComponent<Animator>();
		rb2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        for (int i = 0; i <= 10; i++)
        {
            Instantiate(enemy, new Vector2(i * 5f -26f, 0f), Quaternion.identity);
        }
    }

	// Update is called once per frame
	void Update()
    {
        Controls();

        SetAnimationState();

        RotateWeapon();
    }

    private void Shoot()
    {
        weapon.GetComponent<Animator>().SetBool("isFiring", true);
        canFire = false;
        AddCasing();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        Debug.DrawRay(firingPoint.transform.position, mousePosition - firingPoint.transform.position, Color.white);

        RaycastHit2D hit = Physics2D.Raycast(
            firingPoint.transform.position, 
            mousePosition - firingPoint.transform.position,
            Mathf.Infinity,
            LayerMask.GetMask("Enemy")
        );
        if (hit.collider != null)
        {
            Debug.Log("hit");
            Destroy(hit.transform.gameObject);
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            rb2D.AddForce(Vector3.up * 10f);
        }
    }

    private void Controls()
    {
        Vector2 velocity = rb2D.velocity;
        velocity.x = Input.GetAxis("Horizontal") * 10f;

        if (velocity.x < 0f)
        {
            sprite.flipX = true;
            weapon.GetComponent<SpriteRenderer>().flipY = true;
        }
        if (velocity.x > 0f)
        {
            sprite.flipX = false;
            weapon.GetComponent<SpriteRenderer>().flipY = false;
        }

        if (Input.GetButton("Vertical") && isJumping == false)
        {
            isJumping = true;
            velocity.y = 20f;
        }

        rb2D.velocity = velocity;

        if (Input.GetButtonDown("Fire") && canFire)
        {
            Shoot();
        }

        // Trigger must be released to fire again
        canFire |= Input.GetButtonUp("Fire");
    }

    private void AddCasing()
    {
        GameObject casingClone = Instantiate(casing, weapon.transform.position, Quaternion.identity);
        casingClone.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90f));
        Destroy(casingClone, 5);
    }

    private void RotateWeapon()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angleRad = Mathf.Atan2(
            mousePosition.y - weapon.transform.position.y,
            mousePosition.x - weapon.transform.position.x
        );
        float angleDeg = (180 / Mathf.PI) * angleRad;
        weapon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleDeg));
    }

    private void SetAnimationState()
    {
        if (isJumping)
        {
            animator.SetInteger("State", 2);
            return;
        }

        if (Mathf.Abs(rb2D.velocity.x) > 1f) {
            animator.SetInteger("State", 1);
            return;
        }

        animator.SetInteger("State", 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
	{
		isJumping = false;	
	}
}
