using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotate : MonoBehaviour
{
    float timer = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Purely visual change -> no need for FixedUpdate
        timer += Time.deltaTime;

        var wave1 = 0.52f * Mathf.Sin(timer * 0.31f + 2.42f);
        var wave2 = 4.21f * Mathf.Sin(timer * 0.69f + 0.42f);
        var wave3 = 2.69f * Mathf.Sin(timer * 1.24f + 1.48f);
        var wave4 = 1.69f * Mathf.Sin(timer * 0.53f + 0.99f);

        transform.rotation = new Quaternion(wave1 * wave2, wave1 * wave3, wave2, wave4);
    }
}
