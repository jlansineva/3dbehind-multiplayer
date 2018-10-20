using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummy : Actor
{
    Swingy swingy;

    public GameObject bloodPrefab;

    void Start ()
    {
        swingy = GetComponentInChildren<Swingy>();
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    public override void TakeHit()
    {
        swingy.Impact(Random.insideUnitSphere * 150f);

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
}
