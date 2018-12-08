using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float speed = 10.0F;
    public float rotationSpeed = 100.0F;
    public int forceConst = 15000;
    public int teleportHeightTolerance = 5;
    public int teleportRange = 15;
    public float teleportSpeed = 50f;
    public Transform trans;
    public Animator anim;
    public GameObject targetingPrefab;

    private bool canJump;
    private Rigidbody selfRigidbody;
    private int teleportCalc = 0;
    private bool teleporting = false;
    private bool teleportActive = false;
    private Collision playerCollider;
    private float distToGround;
    private Collider collider;
    private bool isGrounded = true;
    private int isGroundedFramesToWait = 5;
    private Vector3 teleportEndPoint;
    private Vector3 teleportDirection;
    private List<Vector3> teleportPositions = new List<Vector3>();
    private bool teleportTargeting = false;
    private GameObject targetingObject = null;
    int coolDownPeriodInSeconds = 1;
    float timeStamp = 0;

    void Start()
    {
        selfRigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider>();
        distToGround = collider.bounds.extents.y;
    }

    void FixedUpdate()
    {
        isGrounded = IsGrounded();

        if (Input.GetAxis("Jump") > 0 && isGrounded && !teleportActive)
        {
            selfRigidbody.AddForce(0, forceConst, 0, ForceMode.Impulse);
        }

        // Handle WASD movement
        float translationX = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float translationY = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var translation = new Vector3(translationY, 0, translationX);                
        translation = Quaternion.Euler(new Vector3(0, trans.rotation.eulerAngles.y, 0)) * translation;
        translation.y = 0;
        transform.Translate(translation);

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
                teleportPositions.Add(Lerp(transform.position, teleportEndPoint, i/asd));
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
    void Update()
    {
        if (!teleportActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("Hit");
            }

            if (Input.GetMouseButtonDown(1))
            {
                anim.SetTrigger("HitHorizontal");
            }

            if (Input.GetMouseButtonDown(2) && timeStamp <= Time.time && teleportTargeting && isGrounded)
            {
                teleporting = true;
                teleportTargeting = false;
            }

            if (Input.GetKeyDown(KeyCode.E))
                teleportTargeting = !teleportTargeting;
        }
    }
    bool IsGrounded()
    {
        var rayCastPoints = new List<Vector3>
        {
            new Vector3(transform.position.x + 0.9f * collider.bounds.extents.x, transform.position.y, transform.position.z),
            new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.9f * collider.bounds.extents.z),
            new Vector3(transform.position.x - 0.9f * collider.bounds.extents.x, transform.position.y, transform.position.z),
            new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.9f * collider.bounds.extents.z),
            new Vector3(transform.position.x - 0.45f * collider.bounds.extents.x, transform.position.y, transform.position.z - 0.45f * collider.bounds.extents.z),
            new Vector3(transform.position.x - 0.45f * collider.bounds.extents.x, transform.position.y, transform.position.z + 0.45f * collider.bounds.extents.z),
            new Vector3(transform.position.x + 0.45f * collider.bounds.extents.x, transform.position.y, transform.position.z + 0.45f * collider.bounds.extents.z),
            new Vector3(transform.position.x + 0.45f * collider.bounds.extents.x, transform.position.y, transform.position.z + 0.45f * collider.bounds.extents.z),
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