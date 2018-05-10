using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour 
{

	Animator animator;

	public void Start()
	{
		animator = GetComponent<Animator>();	
	}

	public void ResetIsFiring() 
	{
		animator.SetBool("isFiring", false);
	}
}
