using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Security : MonoBehaviour
{
	Animator ani;
	 void Start()
    {
		ani = transform.GetComponent<Animator>();
	}
	void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
         ani.SetBool("PlayerCollider", true);
		}
    }
	void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
         ani.SetBool("PlayerCollider", false);
		}
    }
}
