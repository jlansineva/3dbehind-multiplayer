using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana_rotate : MonoBehaviour {
    public Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("Hitting", true);
        }
        else
        {
            anim.SetBool("Hitting", false);
        }

        if (Input.GetMouseButtonDown(1))
            Debug.Log("Pressed right click.");

        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click.");
    }
}
