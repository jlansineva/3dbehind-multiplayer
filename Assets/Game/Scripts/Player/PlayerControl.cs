using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class PlayerControl : NetworkBehaviour {

    public float speed = 10.0F;
    public float rotationSpeed = 100.0F;
    public int forceConst = 50;
    public Animator anim;

    private bool canJump;
    private Rigidbody selfRigidbody;

    [SyncVar]
    private int teleportCalc = 0;

    [SyncVar]
    private bool teleporting = false;

    void Start()
    {
        if (isLocalPlayer)
        {
            var camera = GameObject.Find("CameraHolder");
            camera.transform.parent = this.transform;
            camera.transform.localPosition = new Vector3(0, 0.8f, 0);
            camera.transform.localRotation = new Quaternion();

            var boi = transform.Find("boi10");
            var characterRenderer = boi.transform.Find("Cube").GetComponent<Renderer>();
            characterRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }

        selfRigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        if (Input.GetAxis("Jump") > 0 && selfRigidbody.position.y < 1)
        {
            selfRigidbody.AddForce(0, forceConst, 0, ForceMode.Impulse);
        }

        float translationX = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float translationY = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var translation = new Vector3(translationY, 0, translationX);
        
        translation = transform.rotation * translation;
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
                var teleport = transform.forward * 1500;
                selfRigidbody.AddForce(teleport);
                teleportCalc++;
            }
        }
    }

    void Update() {
        if (!isLocalPlayer) return;

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
