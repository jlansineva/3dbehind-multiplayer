﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Transform trans;
    private Transform boi;

    [SyncVar]
    private int teleportCalc = 0;

    [SyncVar]
    private bool teleporting = false;

    [SyncVar]
    private float attackTimer = 0;

    [SyncVar]
    private Ray attackDirection = new Ray();

    [SyncVar]
    [SerializeField]
    private int score = 0;

    void Start()
    {
        if (isLocalPlayer)
        {
            var camera = GameObject.Find("CameraHolder");
            camera.transform.parent = this.transform;
            camera.transform.localPosition = new Vector3(0, 0.8f, 0);
            camera.transform.localRotation = new Quaternion();
            trans = camera.transform;

            boi = transform.Find("boi10");
            var characterRenderer = boi.transform.Find("Cube").GetComponent<Renderer>();
            characterRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }

        selfRigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;


    }

    void Update() {
        if (isServer)
        {
            UpdateAttack();
        }

        if (!isLocalPlayer) return;

        if (Input.GetAxis("Jump") > 0 && selfRigidbody.position.y < 1)
        {
            // TODO: should get rid of the rigidbody physics
            selfRigidbody.AddForce(0, forceConst, 0, ForceMode.Impulse);
        }

        float translationX = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float translationY = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var translation = new Vector3(translationY, 0, translationX);

        // Move Y rotation to root
        boi.rotation = Quaternion.Euler(0, trans.rotation.eulerAngles.y, 0);
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

                // TODO: should get rid of the rigidbody physics
                selfRigidbody.AddForce(teleport);
                teleportCalc++;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            CmdAttack();
            anim.SetTrigger("Hit");
        }

        if (Input.GetMouseButtonDown(1))
        {
            CmdAttack();
            anim.SetTrigger("HitHorizontal");
        }

        if (Input.GetMouseButtonDown(2))
        {
            teleporting = true;
        }
    }

    private PlayerControl[] GetOtherPlayers()
    {
        // this might be slow
        var players = FindObjectsOfType<PlayerControl>();

        return players.Where(it => it != this).ToArray();
    }

    private void UpdateAttack()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                var otherPlayers = GetOtherPlayers();
                var impactPoint = attackDirection.origin + attackDirection.direction.normalized * 0.4f;
                var otherPlayerWithinRange = otherPlayers
                    .Where(it => IsWithingRange(it.transform.position, impactPoint, 1.5f));

                foreach (var player in otherPlayerWithinRange)
                {
                    score += 1;
                    player.RpcDead(gameObject);
                }
            }
        }
    }

    private bool IsWithingRange(Vector3 vec1, Vector3 vec2, float distance)
    {
        Debug.Log((vec1 - vec2).sqrMagnitude);
        return (vec1 - vec2).sqrMagnitude < (distance * distance);
    }

    /// <summary>
    /// This is called on server when this player attacks
    /// </summary>
    [Command]
    private void CmdAttack()
    {
        attackTimer = 0.4f;
        attackDirection = new Ray(transform.position, transform.forward);
    }

    /// <summary>
    /// This is called on client after this player dies
    /// </summary>
    [ClientRpc]
    public void RpcDead(GameObject player)
    {
        //anim.SetTrigger("");

        var spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        var randomizedSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        transform.position = randomizedSpawn.transform.position;
        selfRigidbody.velocity = Vector3.zero;
    }
}
