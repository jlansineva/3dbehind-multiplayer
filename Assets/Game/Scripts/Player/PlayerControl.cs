using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float speed = 10.0F;
    public float rotationSpeed = 100.0F;
    public int forceConst = 50;
    public Transform trans;
    public Animator anim;

    private bool canJump;
    private Rigidbody selfRigidbody;
    private int teleportCalc = 0;
    private bool teleporting = false;

    void Start()
    {
        selfRigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (Input.GetAxis("Jump") > 0 && selfRigidbody.position.y < 1)
        {
            selfRigidbody.AddForce(0, forceConst, 0, ForceMode.Impulse);
        }
        float translationX = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float translationY = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var translation = new Vector3(translationY, 0, translationX);
        translation = trans.rotation * translation;
        //float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        //rotation *= Time.deltaTime;
        translation.y = 0;
        transform.Translate(translation);
        //transform.Rotate(0, rotation, 0);

        if (teleporting == true)
        {
            if (teleportCalc == 10)
            {
                teleportCalc = 0;
                teleporting = false;
                selfRigidbody.velocity = Vector3.zero;
            }
            else
            {
                var teleport = trans.forward * 1500;
                selfRigidbody.AddForce(teleport);
                teleportCalc++;
            }
        }
    }
    void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Hit");
        }

        if (Input.GetMouseButtonDown(1))
        {
            anim.SetTrigger("HitHorizontal");
        }

        if (Input.GetMouseButtonDown(2))
        {
            teleporting = true;
        }
    }

}
