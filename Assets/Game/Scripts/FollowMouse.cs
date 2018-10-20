using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public Vector2 sensitivity = Vector2.one;

    private Vector2 look = Vector2.zero;

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        var deltaMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        look += Vector2.Scale(deltaMouse * Time.deltaTime, sensitivity);

        var pitch = Quaternion.Euler(-look.y, 0, 0);
        var yaw = Quaternion.Euler(0, look.x, 0);

        transform.rotation = yaw * pitch;

        if (Input.GetKey(KeyCode.Space))
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
	}
}
