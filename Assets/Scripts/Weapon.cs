using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour 
{   
    Animator animator;

    public GameObject firingPoint;
    public GameObject casing;
    public GameObject clip;
    public GameObject bit;

    public AudioClip gunFire;
    public AudioClip clickSound;
    public AudioClip reloadSound;

    private AudioSource source;

    public bool canFire = true;
    bool isReloading = false;

    public int clipSize = 30;
    int ammo = 0;

    // TODO: Damage should be relative to distance
    public float damage = 1;

	public void Start()
	{
		animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        Reload();
	}

    public void RotateWeapon(Vector3 target)
    {
        float angleDeg = (180 / Mathf.PI) * Mathf.Atan2(
            target.y - transform.position.y,
            target.x - transform.position.x
        );
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleDeg));
    }

    private void AddCasing()
    {
        GameObject casingClone = Instantiate(casing, transform.position, Quaternion.identity);
        Vector3 v = Quaternion.AngleAxis(Random.Range(270f, 360f), Vector3.forward) * Vector3.up;
        casingClone.GetComponent<Rigidbody2D>().velocity = v * 10f;
        Destroy(casingClone, 1);
    }

    public void Shoot()
    {
        if (isReloading == true || canFire == false) {
            return;
        }

        if (ammo <= 0) {
            source.PlayOneShot(clickSound);
            return;
        }

        source.PlayOneShot(gunFire, 1f);
        animator.SetTrigger("Fire");
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

            for (int i = 0; i <= 50; i++) {
                GameObject bitClone = Instantiate(bit, hit.point, Quaternion.identity);
                Vector3 velocity = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * Vector3.up;
                bitClone.GetComponent<Rigidbody2D>().velocity = velocity * Random.Range(-10f, 10f);
                Destroy(bitClone, 1);
            }

            if (hit.collider is CircleCollider2D) {
                hit.collider.gameObject.GetComponent<Bunny>().Hurt(angleDeg, 3f);
                hit.collider.gameObject.GetComponent<Rigidbody2D>().velocity = v * 10f;
            } else if (hit.collider is BoxCollider2D && hit.collider.gameObject.GetComponent<Bunny>() != null) {
                hit.collider.gameObject.GetComponent<Bunny>().Hurt(angleDeg, 1f);
                hit.collider.gameObject.GetComponent<Rigidbody2D>().velocity = v * 10f;
            }

        }
    }


    public void DropClip()
    {
        GameObject clipClone = Instantiate(clip, transform.position, Quaternion.identity);
        Destroy(clipClone, 10);
    }

    public void Reload()
    {
        if (isReloading == false) {
            isReloading = true;
            DropClip();
            source.PlayOneShot(reloadSound);
            animator.SetBool("isReloading", true);
        }
    }

    public void Reloaded() {
        ammo = clipSize;
        animator.SetBool("isReloading", false);                
        isReloading = false;
    }
}
