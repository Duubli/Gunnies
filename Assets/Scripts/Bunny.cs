using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour
{   
	Animator animator;
    Rigidbody2D rb2D;
    SpriteRenderer sprite;

    public AudioClip gunFire;
    public AudioClip clickSound;
    public AudioClip reloadSound;

    public GameObject weapon;
    public GameObject casing;
    public GameObject enemy;
    public GameObject firingPoint;
    public GameObject bit;
    public GameObject clip;
     
    private AudioSource source;
   
	bool canFire = true;
    bool isJumping = true;
    bool isReloading = false;

    float timer = 0f;

    public int ammo = 30;

	// Use this for initialization
	void Start()
	{
        source = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		rb2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

	// Update is called once per frame
	void Update()
    {
        Controls();

        SetAnimationState();
    }

    private void FixedUpdate()
    {
        timer--;
        if (timer <= 0f) {
            timer = 50f;
            Instantiate(enemy, new Vector2(Random.Range(0f, 10f) * 5f - 26f, 0f), Quaternion.identity);
        }
    }

    private void Shoot()
    {
        source.PlayOneShot(gunFire, 1f);
        weapon.GetComponent<Animator>().SetTrigger("Fire");
        canFire = false;
        AddCasing();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.DrawRay(firingPoint.transform.position, mousePosition - firingPoint.transform.position, Color.white);

        RaycastHit2D hit = Physics2D.Raycast(
            firingPoint.transform.position, 
            mousePosition - firingPoint.transform.position,
            Mathf.Infinity,
            LayerMask.GetMask("Enemy", "Background")
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

            for (int i = 0; i <= 50; i++)
            {
                GameObject bitClone = Instantiate(bit, hit.point, Quaternion.identity);
                Vector3 velocity = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * Vector3.up;
                bitClone.GetComponent<Rigidbody2D>().velocity = velocity * Random.Range(-10f, 10f);
                Destroy(bitClone, 1);
            }

            if (hit.collider is BoxCollider2D && hit.collider.gameObject.name == "Enemy") {
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
            if (ammo > 0) {
                Shoot();
                ammo--;
                isReloading = false;
            } else {
                source.PlayOneShot(clickSound);
            }
        }

        if (Input.GetButton("Reload") && isReloading == false) {
            GameObject clipClone = Instantiate(clip, transform.position, Quaternion.identity);
            Destroy(clipClone, 10);
            weapon.GetComponent<Animator>().SetBool("isReloading", true);
            isReloading = true;
            ammo = 30;
            source.PlayOneShot(reloadSound);
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
