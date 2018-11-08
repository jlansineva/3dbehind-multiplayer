using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBall : MonoBehaviour
{
    public Color color;

	// Use this for initialization
	void Start ()
    {
        GetComponent<Renderer>().material.color = color;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (transform.position.y <= -10 )
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var splatterTarget = collision.gameObject.GetComponent<SplatterTarget>();

        if (splatterTarget != null)
        {
            var contant = collision.contacts[0];
            splatterTarget.DrawCircle(contant.point, contant.normal, transform.lossyScale.x, color);
            Destroy(gameObject);
        }
    }
}
