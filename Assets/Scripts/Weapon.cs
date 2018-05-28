using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour 
{   
	Animator animator;
    public GameObject firingPoint;

	public void Start()
	{
		animator = GetComponent<Animator>();
	}

    public void Update()
    {
        RotateWeapon();
    }

    private void RotateWeapon()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angleRad = Mathf.Atan2(
            mousePosition.y - transform.position.y,
            mousePosition.x - transform.position.x
        );
        float angleDeg = (180 / Mathf.PI) * angleRad;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleDeg));
    }

    public void ResetIsFiring() 
	{
		animator.SetBool("isFiring", false);
	}
}
