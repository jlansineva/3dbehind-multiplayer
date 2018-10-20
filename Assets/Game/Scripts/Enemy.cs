using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public string enemyName = "boi";

    public GameObject target;
    public Animator anim;
    public GameObject bloodPrefab;

    public int hp = 4;
    public int iframes = 60;
    public bool hit = false;

    GameObject leftArm;
    GameObject rightArm;
    bool attacking = false;

    GameObject model;

    void Start ()
    {
        leftArm = transform.Find("LeftHand").gameObject;
        rightArm = transform.Find("RightHand").gameObject;
        model = transform.Find("boi10").gameObject;
    }

    IEnumerator IframeCalc()
    {
        yield return new WaitUntil(() => iframes == 0);
        if (hp > 2)
        {
            hp -= 2;
        }
        else Die();

        iframes = 60;
        hit = false;
    }

    void Update ()
    {
        if (hit)
        {
            iframes--;
        }

        if (target != null)
        {
            var targetDir = target.transform.position - transform.position;
            var targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);

            transform.Translate(Vector3.forward * 4 * Time.deltaTime);

            if (model) model.GetComponent<Animator>().SetBool("moving", true);

            if (Vector3.Distance(target.transform.position, transform.position) < 1.5f && !attacking)
            {
                StartCoroutine("Attack");
            }
        } 
	}

    public override void Die()
    {
        if (model) model.GetComponent<Animator>().SetBool("dead", true);
        target = null;
        var selfRigidBody = GetComponent<Rigidbody>();
        selfRigidBody.AddForce((-transform.forward *3 + Vector3.up * 3), ForceMode.VelocityChange);
        selfRigidBody.constraints = RigidbodyConstraints.None;
    }

    public override void TakeHit()
    {
        StartCoroutine(IframeCalc());
        hit = true;

        for (int i = 0; i < 14; i++)
        {
            var momentum = Vector3.Scale(Random.insideUnitSphere, new Vector3(1.5f, 6f, 1.5f));
            CreateBlood(Random.Range(0.04f, 0.4f), momentum);
        }
    }

    void CreateBlood(float size, Vector3 momentum)
    {
        var blood = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
        blood.transform.localScale = Vector3.one * size;
        blood.transform.position += momentum * .1f + Vector3.up * .2f;
        blood.GetComponent<Rigidbody>().AddForce(momentum, ForceMode.Impulse);
    }

    IEnumerator Attack()
    {
        attacking = true;
        if (model) model.GetComponent<Animator>().SetBool("attacking", true);

        for (float f = 0f; f <= 40f; f += 2.5f)
        {
            SetArmAngle(f);
            yield return null;
        }

        float a = 1f;
        for (float f = 40f; f >= 0f; f -= a)
        {
            a += 0.8f;
            SetArmAngle(f);
            yield return null;
        }

        var impactPoint = transform.position + transform.forward * 0.6f;
        foreach (var collider in Physics.OverlapSphere(impactPoint, 0.5f))
        {
            var actor = collider.GetComponent<Actor>();

            if (actor != null && actor != this)
            {
                actor.TakeHit();
            }
        }

        attacking = false;
    }

    void SetArmAngle(float angle, bool left = true, bool right = true)
    {
        if (left) leftArm.transform.localEulerAngles = new Vector3(-angle, 0f, 0f);
        if (right) rightArm.transform.localEulerAngles = new Vector3(-angle, 0f, 0f);
    }
}
