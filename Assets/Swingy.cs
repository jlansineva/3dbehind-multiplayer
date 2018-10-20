using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swingy : MonoBehaviour
{
    [Range(0.01f, 10f)]
    public float rigidness = 5f;

    [Range(0f, 1f)]
    public float friction = 0.05f;

    public Vector2 position;
    public Vector2 deltaPosition;

    public void Impact(Vector3 impact)
    {
        deltaPosition += new Vector2(impact.x, impact.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        position += deltaPosition * Time.deltaTime;
        deltaPosition *= (1 - friction);
        deltaPosition -= position * (1 / rigidness);
        transform.rotation = Quaternion.Euler(position.magnitude, Vector2.SignedAngle(position, Vector2.up), 0);
    }
}
