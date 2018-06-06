using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour {

    public AudioClip hurtSound;

    public float health = 3;

    public GameObject particle;
    public GameObject weapon;
    public Rigidbody2D rb2D;

    public GameObject weaponClone;

    public bool isJumping = true;

    bool dead = false;

    Weapon weaponScript;
    SpriteRenderer sprite;

    Animator animator;

    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        weaponScript = weapon.GetComponent<Weapon>();

        EquipWeapon();
    }

    void Update()
    {
        SetDirection();
        SetAnimationState();
    }

    void FixedUpdate() 
    {
        Heal();
    }

    void Heal()
    {
        if (health < 3f && health > 0f) {
            health += .01f;
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb2D.velocity = velocity;    
    }

    private void SetDirection()
    {
        if (rb2D.velocity.x < 0f) {
            sprite.flipX = true;
            weaponClone.GetComponent<SpriteRenderer>().flipY = true;
        }
        if (rb2D.velocity.x > 0f) {
            sprite.flipX = false;
            weaponClone.GetComponent<SpriteRenderer>().flipY = false;
        }   
    }

    private void EquipWeapon()
    {
        weaponClone = Instantiate(weapon, transform.position, Quaternion.identity);
        weaponClone.transform.parent = gameObject.transform;
        Vector3 weaponPosition = new Vector3(
            weaponClone.transform.position.x,
            weaponClone.transform.position.y - 0.35f,
            weaponClone.transform.position.z
        );
        weaponClone.transform.position = weaponPosition;
    }

    private void SetAnimationState()
    {
        if (dead) {
            return;
        }

        if (isJumping)
        {
            animator.SetInteger("State", 2);
            return;
        }

        if (Mathf.Abs(rb2D.velocity.x) > 1f)
        {
            animator.SetInteger("State", 1);
            return;
        }

        animator.SetInteger("State", 0);
    }

    public void Hurt (float angle, float damage)
    {
        animator.SetTrigger("Hurt");
        source.PlayOneShot(hurtSound);
        health -= damage;
        if (health <= 0f) {
            Destroy(weaponClone);
            animator.SetInteger("State", -1);
            gameObject.layer = 9;
            dead = true;
            return;
        }
        for (int i = 0; i <= 50; i++) {
            GameObject particleClone = Instantiate(particle, transform.position, Quaternion.identity);
            Vector3 v = Quaternion.AngleAxis(angle + Random.Range(-45f, 45f), Vector3.forward) * Vector3.up;
            particleClone.GetComponent<Rigidbody2D>().velocity = v * Random.Range(-10f, 10f);
            Destroy(particleClone, 1);
        }
    }

    private void OnCollisionEnter2D()
    {
        isJumping = false;
    }
}
