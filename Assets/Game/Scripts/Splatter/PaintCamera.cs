using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCamera : MonoBehaviour
{
    public GameObject paintBallPrefab;
    private new Camera camera;

	// Use this for initialization
	void Start ()
    {
        camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if (Input.GetMouseButton(0)) Paint();

        if (Input.GetMouseButtonDown(0)) ShootPaintBall();
	}

    private void ShootPaintBall()
    {
        var newObject = Instantiate(paintBallPrefab, transform.position, transform.rotation);
        newObject.transform.localScale = Vector3.one * Random.Range(0.1f, 0.5f);
        newObject.GetComponent<Rigidbody>().AddForce(transform.forward * 400f);
    }

    private void Paint()
    {
        var rayOrigin = camera.transform.position;

        RaycastHit hit;
        if (!Physics.Raycast(rayOrigin, camera.transform.forward, out hit))
            return;
            
        var target = hit.transform.gameObject;

        var splatterTarget = target.GetComponent<SplatterTarget>();
        if (splatterTarget == null)
            return;

        splatterTarget.DrawCircle(hit.textureCoord, .2f, Color.red);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 250, 20), "WASD - movement");
        GUI.Label(new Rect(5, 25, 250, 20), "Space (hold) - restrain cursor");
        GUI.Label(new Rect(5, 45, 250, 20), "Mouse1 - shoot");
    }
}
