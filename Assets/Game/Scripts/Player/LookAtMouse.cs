using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour {
    private Vector2 look = Vector2.zero;
    public float sensitivity = 133.7f;
	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
        look += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;

        look.y = Mathf.Clamp(look.y, -90, 90);
		
        var pitch = Quaternion.Euler(-look.y, 0, 0);
        var yaw = Quaternion.Euler(0, look.x, 0);

        transform.rotation = yaw * pitch;
	}
}
