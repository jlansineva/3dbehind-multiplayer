using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class PlayerControl : NetworkBehaviour {

    public float speed = 10.0F;
    public float rotationSpeed = 100.0F;
    public int forceConst = 15000;
    public int teleportHeightTolerance = 5;
    public int teleportRange = 15;
    public float teleportSpeed = 50f;
    public Transform trans;
    public Animator anim;
    public GameObject targetingPrefab;

    private Rigidbody selfRigidbody;
    private Transform boi;

    [SyncVar]
    private bool teleporting = false;
    private bool teleportActive = false;
    private float distToGround;
    private Collider playerCollider;
    private bool isGrounded = true;
    private int isGroundedFramesToWait = 5;
    private Vector3 teleportEndPoint;
    private Vector3 teleportDirection;
    private List<Vector3> teleportPositions = new List<Vector3>();
    private bool teleportTargeting = false;
    private GameObject targetingObject = null;
    int coolDownPeriodInSeconds = 1;
    float timeStamp = 0;

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
        playerCollider = GetComponent<Collider>();
        distToGround = playerCollider.bounds.extents.y;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        isGrounded = IsGrounded();

        if (Input.GetAxis("Jump") > 0 && isGrounded && !teleportActive)
        {
            // TODO: should get rid of the rigidbody physics
            selfRigidbody.AddForce(0, forceConst, 0, ForceMode.Impulse);
        }

        if (teleporting == true)
        {
            teleporting = false;
            timeStamp = Time.time + coolDownPeriodInSeconds;
            teleportEndPoint = GetTeleportEndPoint();
            teleportDirection = teleportEndPoint - transform.position;
            var asd = (float)Math.Ceiling(teleportDirection.magnitude / teleportSpeed);
            teleportPositions = new List<Vector3>
            {
                transform.position
            };
            for (int i = 1; i < asd; i++)
            {
                teleportPositions.Add(Lerp(transform.position, teleportEndPoint, i / asd));
            }

            teleportPositions.Add(teleportEndPoint);

            selfRigidbody.useGravity = false;

            teleportActive = true;
        }

        if (teleportActive)
        {
            transform.position = teleportPositions.First();
            teleportPositions.RemoveAt(0);
            if (teleportPositions.Count == 0)
            {
                selfRigidbody.useGravity = true;
                teleportActive = false;
            }
        }

        if (teleportTargeting)
        {
            var endPoint = GetTeleportEndPoint();
            if (targetingObject == null)
                targetingObject = Instantiate(targetingPrefab, endPoint + new Vector3(0, distToGround, 0), new Quaternion());
            else
                targetingObject.transform.position = endPoint + new Vector3(0, distToGround, 0);
        }
        else
        {
            Destroy(targetingObject);
            targetingObject = null;
        }
    }

    void Update() {
        if (isServer)
        {
            UpdateAttack();
        }

        if (!isLocalPlayer) return;

        // Handle WASD movement
        float translationX = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float translationY = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var translation = new Vector3(translationY, 0, translationX);
        translation = Quaternion.Euler(new Vector3(0, trans.rotation.eulerAngles.y, 0)) * translation;
        translation.y = 0;
        transform.Translate(translation);

        // Move Y rotation to root
        boi.rotation = Quaternion.Euler(0, trans.rotation.eulerAngles.y, 0);       

        if (!teleportActive)
        {
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

            if (Input.GetMouseButtonDown(2) && timeStamp <= Time.time && teleportTargeting && isGrounded)
            {
                teleporting = true;
                teleportTargeting = false;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                teleportTargeting = !teleportTargeting;
            }
        }
    }

    bool IsGrounded()
    {
        var rayCastPoints = new List<Vector3>
        {
            new Vector3(transform.position.x + 0.9f * playerCollider.bounds.extents.x, transform.position.y, transform.position.z),
            new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.9f * playerCollider.bounds.extents.z),
            new Vector3(transform.position.x - 0.9f * playerCollider.bounds.extents.x, transform.position.y, transform.position.z),
            new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.9f * playerCollider.bounds.extents.z),
            new Vector3(transform.position.x - 0.45f * playerCollider.bounds.extents.x, transform.position.y, transform.position.z - 0.45f * playerCollider.bounds.extents.z),
            new Vector3(transform.position.x - 0.45f * playerCollider.bounds.extents.x, transform.position.y, transform.position.z + 0.45f * playerCollider.bounds.extents.z),
            new Vector3(transform.position.x + 0.45f * playerCollider.bounds.extents.x, transform.position.y, transform.position.z + 0.45f * playerCollider.bounds.extents.z),
            new Vector3(transform.position.x + 0.45f * playerCollider.bounds.extents.x, transform.position.y, transform.position.z + 0.45f * playerCollider.bounds.extents.z),
            transform.position
        };

        if (CheckGroundedStatus(rayCastPoints))
        {
            isGroundedFramesToWait = 5;
            return true;
        }
        else if (isGroundedFramesToWait > 0)
        {
            isGroundedFramesToWait--;
            return true;
        }
        else return false;
    }

    bool CheckGroundedStatus(List<Vector3> rayCastPoints)
    {
        RaycastHit hit;
        Vector3 dir = new Vector3(0, -1);

        return rayCastPoints.Any(x => Physics.Raycast(x, dir, out hit, distToGround + 0.1f));
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
        var randomizedSpawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        transform.position = randomizedSpawn.transform.position;
        selfRigidbody.velocity = Vector3.zero;
    }

    Vector3 GetTeleportEndPoint()
    {
        RaycastHit hit;
        Vector3 dir = transform.rotation.eulerAngles;
        var camera = GameObject.Find("CameraHolder");
        dir = camera.transform.forward;
        dir.Normalize();

        if (Physics.Raycast(transform.position, dir, out hit, teleportRange))
            return hit.point;
        else
        {
            var endPoint = transform.position + dir * 5;
            for (int i = teleportRange * 10; i > 0; i--)
            {
                if (Physics.Raycast(LerpByDistance(transform.position, endPoint, i / 10f), Vector3.down, out hit, teleportHeightTolerance))
                    return hit.point;
            }
            return transform.position;
        }
    }

    Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }

    Vector3 Lerp(Vector3 start, Vector3 end, float percent)
    {
        return (start + percent * (end - start));
    }
}

