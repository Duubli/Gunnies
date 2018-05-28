using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour
{   
	Animator animator;
    Rigidbody2D rb2D;
    SpriteRenderer sprite;

    public AudioClip gunFire;
    public GameObject weapon;
    public GameObject casing;
    public GameObject enemy;
    public GameObject firingPoint;

    private AudioSource source;
   
	bool canFire = true;
    bool isJumping = true;

	// Use this for initialization
	void Start()
	{
        source = GetComponent<AudioSource>();
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
    }

    private void Shoot()
    {
        source.PlayOneShot(gunFire, 1f);
        weapon.GetComponent<Animator>().SetBool("isFiring", true);
        canFire = false;
        AddCasing();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.DrawRay(firingPoint.transform.position, mousePosition - firingPoint.transform.position, Color.white);

        RaycastHit2D hit = Physics2D.Raycast(
            firingPoint.transform.position, 
            mousePosition - firingPoint.transform.position,
            Mathf.Infinity,
            LayerMask.GetMask("Enemy")
        );

        if (hit.collider != null)
        {
            float angleRad = Mathf.Atan2(
                mousePosition.y - transform.position.y,
                mousePosition.x - transform.position.x
            );
            float angleDeg = (180 / Mathf.PI) * angleRad;
            Vector3 v = Quaternion.AngleAxis(angleDeg - 90f, Vector3.forward) * Vector3.up;
            // TODO: Make the force relative to damage
            hit.collider.gameObject.GetComponent<Rigidbody2D>().velocity = v * 10f;
            if (hit.collider is BoxCollider2D) {
                hit.collider.gameObject.GetComponent<Enemy>().Hurt(angleDeg, 1f);    
            }
            if (hit.collider is CircleCollider2D)
            {
                hit.collider.gameObject.GetComponent<Enemy>().Hurt(angleDeg, 3f); 
            }

        }
    }

    private void Controls()
    {
        Vector2 velocity = rb2D.velocity;
        velocity.x = Input.GetAxis("Horizontal") * 20f;

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
            velocity.y = 30f;
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
        Vector3 v = Quaternion.AngleAxis(Random.Range(270f, 360f), Vector3.forward) * Vector3.up;
        casingClone.GetComponent<Rigidbody2D>().velocity = v * 10f;
        Destroy(casingClone, 1);
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
