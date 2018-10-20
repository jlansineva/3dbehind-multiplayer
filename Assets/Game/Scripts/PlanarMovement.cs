using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarMovement : MonoBehaviour {

    public float speed = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        var forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        var right = Vector3.ProjectOnPlane(transform.right, Vector3.up);

        var mov = (Input.GetAxis("Horizontal") * right + Input.GetAxis("Vertical") * forward) * speed * Time.deltaTime;

        transform.position += mov;
	}
}
